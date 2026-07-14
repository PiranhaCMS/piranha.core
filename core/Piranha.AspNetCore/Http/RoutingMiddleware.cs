/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Piranha.AspNetCore.Services;
using Piranha.Models;

namespace Piranha.AspNetCore.Http;

/// <summary>
/// The main application middleware.
/// </summary>
public class RoutingMiddleware : MiddlewareBase
{
    private readonly RoutingOptions _options;

    /// <summary>
    /// Creates a new middleware instance.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline</param>
    /// <param name="options">The current routing options</param>
    /// <param name="factory">The logger factory</param>
    public RoutingMiddleware(RequestDelegate next, IOptions<RoutingOptions> options, ILoggerFactory factory = null) : base(next, factory)
    {
        _options = options.Value;
    }

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="context">The current http context</param>
    /// <param name="api">The current api</param>
    /// <param name="service">The application service</param>
    /// <returns>An async task</returns>
    public override async Task Invoke(HttpContext context, IApi api, IApplicationService service)
    {
        if (IsHandled(context) || IsManagerRequest(context.Request.Path.Value))
        {
            await _next.Invoke(context);
            return;
        }

        await RouteRequestAsync(context, api, service).ConfigureAwait(false);
    }

    private async Task RouteRequestAsync(HttpContext context, IApi api, IApplicationService service)
    {
        if (IsHandled(context) || IsManagerRequest(context.Request.Path.Value))
        {
            await _next.Invoke(context);
            return;
        }

        var state = InitializeRoutingState(context, api, service);

        if (!await ResolveSiteAndLanguageAsync(state).ConfigureAwait(false) ||
            (state.Segments.Length <= state.Position && !_options.UseStartpageRouting))
        {
            await _next.Invoke(context);
            return;
        }

        if (await TryHandleAliasAsync(state).ConfigureAwait(false))
        {
            return;
        }

        if (!await ResolvePageAsync(state).ConfigureAwait(false))
        {
            await _next.Invoke(context);
            return;
        }

        await ResolvePostAsync(state).ConfigureAwait(false);
        LogContent(state);

        if (!await BuildRouteAsync(state).ConfigureAwait(false))
        {
            return;
        }

        ApplyRoute(state);
        await _next.Invoke(context);
    }

    private RoutingState InitializeRoutingState(HttpContext context, IApi api, IApplicationService service)
    {
        var url = context.Request.Path.HasValue ? context.Request.Path.Value : "";
        var segments = !string.IsNullOrEmpty(url)
            ? url.Substring(1).Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
            : new string[] { };

        _logger?.LogDebug($"Url: [{ url }]");

        service.Request.Url = context.Request.Path.Value;
        service.Request.Host = context.Request.Host.Host;
        service.Request.Port = context.Request.Host.Port;
        service.Request.Scheme = context.Request.Scheme;

        return new RoutingState
        {
            Context = context,
            Api = api,
            Service = service,
            Config = new Config(api),
            Segments = segments,
            Hostname = context.Request.Host.Host
        };
    }

    private async Task<bool> ResolveSiteAndLanguageAsync(RoutingState state)
    {
        if (_options.UseSiteRouting && state.Segments.Length > 0)
        {
            var prefixedHostname = $"{state.Hostname}/{state.Segments[0]}";
            state.Site = await state.Api.Sites.GetByHostnameAsync(prefixedHostname).ConfigureAwait(false);

            if (state.Site != null)
            {
                state.Context.Request.Path = "/" + string.Join("/", state.Segments.Skip(1));
                state.Hostname = prefixedHostname;
                state.Position = 1;
            }
        }

        if (_options.UseSiteRouting && state.Site == null)
        {
            state.Site = await state.Api.Sites.GetByHostnameAsync(state.Context.Request.Host.Host).ConfigureAwait(false);
        }

        state.Site ??= await state.Api.Sites.GetDefaultAsync().ConfigureAwait(false);
        if (state.Site == null)
        {
            return false;
        }

        var hostnameLanguage = await state.Api.Languages.GetByHostnameAsync(state.Context.Request.Host.Host).ConfigureAwait(false);
        var language = hostnameLanguage ?? await state.Api.Languages.GetByIdAsync(state.Site.LanguageId).ConfigureAwait(false);
        state.LanguageId = language?.Id;

        state.Service.Site.Id = state.Site.Id;
        state.Service.Site.LanguageId = language?.Id ?? Guid.Empty;
        state.Service.Site.Culture = language?.Culture;

        var siteHost = GetMatchingHost(state.Site, state.Hostname);
        state.Service.Site.Host = siteHost[0];
        state.Service.Site.SitePrefix = siteHost[1];
        state.Service.Site.Description.Title = state.Site.Title;
        state.Service.Site.Description.Body = state.Site.Description;
        state.Service.Site.Description.Logo = state.Site.Logo;

        if (string.IsNullOrEmpty(state.Service.Site.Host))
        {
            state.Service.Site.Host = state.Context.Request.Host.Host;
        }

        if (state.Segments.Length > state.Position)
        {
            var languages = await state.Api.Languages.GetAllAsync().ConfigureAwait(false);
            var matchedLanguage = languages.FirstOrDefault(l =>
                l.Id != language?.Id &&
                !string.IsNullOrEmpty(l.Culture) &&
                (l.Culture.Split('-', '_')[0].Equals(state.Segments[state.Position], StringComparison.OrdinalIgnoreCase) ||
                 l.Culture.Equals(state.Segments[state.Position], StringComparison.OrdinalIgnoreCase) ||
                 l.Culture.Replace('-', '_').Equals(state.Segments[state.Position], StringComparison.OrdinalIgnoreCase)));

            if (matchedLanguage != null)
            {
                state.LanguageId = matchedLanguage.Id;
                state.Service.Site.LanguageId = matchedLanguage.Id;
                state.Service.Site.Culture = matchedLanguage.Culture;
                state.Position++;
            }
        }

        state.Service.Site.Sitemap = await state.Api.Sites.GetSitemapAsync(state.Site.Id, true, state.LanguageId).ConfigureAwait(false);

        if (!string.IsNullOrEmpty(state.Service.Site.Culture))
        {
            var cultureInfo = new CultureInfo(state.Service.Site.Culture);
            CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = cultureInfo;
        }

        return true;
    }

    private async Task<bool> TryHandleAliasAsync(RoutingState state)
    {
        if (_options.UseAliasRouting && state.Segments.Length > state.Position)
        {
            var alias = await state.Api.Aliases.GetByAliasUrlAsync(
                $"/{ string.Join("/", state.Segments.Subset(state.Position)) }", state.Service.Site.Id);

            if (alias != null)
            {
                state.Context.Response.Redirect(alias.RedirectUrl, alias.Type == RedirectType.Permanent);
                return true;
            }
        }

        return false;
    }

    private async Task<bool> ResolvePageAsync(RoutingState state)
    {
        if (state.Segments.Length > state.Position)
        {
            for (var n = state.Segments.Length; n > state.Position; n--)
            {
                var slug = string.Join("/", state.Segments.Subset(state.Position, n - state.Position));
                state.Page = await state.Api.Pages.GetBySlugAsync<PageBase>(slug, state.Site.Id, state.LanguageId).ConfigureAwait(false);

                if (state.Page != null)
                {
                    state.Position = n;
                    break;
                }
            }
        }
        else
        {
            state.Page = await state.Api.Pages.GetStartpageAsync<PageBase>(state.Site.Id, state.LanguageId).ConfigureAwait(false);
        }

        if (state.Page == null)
        {
            return true;
        }

        if (!state.Page.IsPublished &&
            (!state.Context.Request.Query.ContainsKey("draft") || state.Context.Request.Query["draft"] != "true"))
        {
            return false;
        }

        state.PageType = App.PageTypes.GetById(state.Page.TypeId);
        state.Service.PageId = state.Page.Id;

        if (state.Page.IsPublished)
        {
            state.Service.CurrentPage = state.Page;
        }

        return true;
    }

    private async Task ResolvePostAsync(RoutingState state)
    {
        if (!_options.UsePostRouting || state.Page == null || !state.PageType.IsArchive || state.Segments.Length <= state.Position)
        {
            return;
        }

        state.Post = await state.Api.Posts.GetBySlugAsync<PostBase>(state.Page.Id, state.Segments[state.Position]).ConfigureAwait(false);
        if (state.Post == null)
        {
            return;
        }

        state.Position++;
        App.PostTypes.GetById(state.Post.TypeId);

        if (state.Post.IsPublished)
        {
            state.Service.CurrentPost = state.Post;
        }
    }

    private void LogContent(RoutingState state)
    {
        _logger?.LogDebug($"Found Site: [{ state.Site.Id }]");
        if (state.Page != null)
        {
            _logger?.LogDebug($"Found Page: [{ state.Page.Id }]");
        }

        if (state.Post != null)
        {
            _logger?.LogDebug($"Found Post: [{ state.Post.Id }]");
        }
    }

    private async Task<bool> BuildRouteAsync(RoutingState state)
    {
        if (state.Post != null)
        {
            if (!string.IsNullOrWhiteSpace(state.Post.RedirectUrl))
            {
                _logger?.LogDebug($"Setting redirect: [{ state.Post.RedirectUrl }]");
                state.Context.Response.Redirect(state.Post.RedirectUrl, state.Post.RedirectType == RedirectType.Permanent);
                return false;
            }

            if (HandleCache(state.Context, state.Site, state.Post, state.Config.CacheExpiresPosts))
            {
                return false;
            }

            state.Route.Append(state.Post.Route ?? "/post");
            AppendTrailingSegments(state);
            state.Query.Append("id=");
            state.Query.Append(state.Post.Id);
            return true;
        }

        if (state.Page == null || !_options.UsePageRouting)
        {
            return true;
        }

        if (!string.IsNullOrWhiteSpace(state.Page.RedirectUrl))
        {
            _logger?.LogDebug($"Setting redirect: [{ state.Page.RedirectUrl }]");
            state.Context.Response.Redirect(state.Page.RedirectUrl, state.Page.RedirectType == RedirectType.Permanent);
            return false;
        }

        state.Route.Append(state.Page.Route ?? (state.PageType.IsArchive ? "/archive" : "/page"));
        state.Query.Append("id=");
        state.Query.Append(state.Page.Id);

        if (!state.Page.ParentId.HasValue && state.Page.SortOrder == 0)
        {
            state.Query.Append("&startpage=true");
        }

        if (!state.PageType.IsArchive || !_options.UseArchiveRouting)
        {
            if (HandleCache(state.Context, state.Site, state.Page, state.Config.CacheExpiresPages))
            {
                return false;
            }

            AppendTrailingSegments(state);
        }
        else if (state.Post == null)
        {
            await AppendArchiveParametersAsync(state).ConfigureAwait(false);
        }

        return true;
    }

    private void AppendTrailingSegments(RoutingState state)
    {
        for (var n = state.Position; n < state.Segments.Length; n++)
        {
            state.Route.Append("/");
            state.Route.Append(state.Segments[n]);
        }
    }

    private async Task AppendArchiveParametersAsync(RoutingState state)
    {
        int? year = null;
        bool foundCategory = false;
        bool foundTag = false;
        bool foundPage = false;

        for (var n = state.Position; n < state.Segments.Length; n++)
        {
            if (state.Segments[n] == "category" && !foundPage)
            {
                foundCategory = true;
                continue;
            }

            if (state.Segments[n] == "tag" && !foundPage && !foundCategory)
            {
                foundTag = true;
                continue;
            }

            if (state.Segments[n] == "page")
            {
                foundPage = true;
                continue;
            }

            state.FoundCategory = foundCategory;
            state.FoundTag = foundTag;
            await AppendArchiveFilterAsync(state, n).ConfigureAwait(false);
            foundCategory = state.FoundCategory;
            foundTag = state.FoundTag;

            if (foundPage)
            {
                try
                {
                    var pageNum = Convert.ToInt32(state.Segments[n]);
                    state.Query.Append("&page=");
                    state.Query.Append(pageNum);
                    state.Query.Append("&pagenum=");
                    state.Query.Append(pageNum);
                }
                catch
                {
                    // We don't care about the exception, we just discard malformed input.
                }
                break;
            }

            AppendArchiveDate(state, n, ref year);
        }
    }

    private async Task AppendArchiveFilterAsync(RoutingState state, int index)
    {
        if (state.FoundCategory)
        {
            try
            {
                var categoryId = (await state.Api.Posts.GetCategoryBySlugAsync(state.Page.Id, state.Segments[index]).ConfigureAwait(false))?.Id;
                if (categoryId.HasValue)
                {
                    state.Query.Append("&category=");
                    state.Query.Append(categoryId);
                }
            }
            finally
            {
                state.FoundCategory = false;
            }
        }

        if (state.FoundTag)
        {
            try
            {
                var tagId = (await state.Api.Posts.GetTagBySlugAsync(state.Page.Id, state.Segments[index]).ConfigureAwait(false))?.Id;
                if (tagId.HasValue)
                {
                    state.Query.Append("&tag=");
                    state.Query.Append(tagId);
                }
            }
            finally
            {
                state.FoundTag = false;
            }
        }
    }

    private void AppendArchiveDate(RoutingState state, int index, ref int? year)
    {
        if (!year.HasValue)
        {
            try
            {
                year = Convert.ToInt32(state.Segments[index]);
                if (year.Value > DateTime.Now.Year)
                {
                    year = DateTime.Now.Year;
                }
                state.Query.Append("&year=");
                state.Query.Append(year);
            }
            catch
            {
                // We don't care about the exception, we just discard malformed input.
            }
        }
        else
        {
            try
            {
                var month = Math.Max(Math.Min(Convert.ToInt32(state.Segments[index]), 12), 1);
                state.Query.Append("&month=");
                state.Query.Append(month);
            }
            catch
            {
                // We don't care about the exception, we just discard malformed input.
            }
        }
    }

    private void ApplyRoute(RoutingState state)
    {
        if (state.Route.Length == 0)
        {
            return;
        }

        var strRoute = state.Route.ToString();
        var strQuery = state.Query.ToString();
        _logger?.LogDebug($"Setting Route: [{ strRoute }?{ strQuery }]");

        state.Context.Request.Path = new PathString(strRoute);
        state.Context.Request.QueryString = state.Context.Request.QueryString.HasValue
            ? new QueryString(state.Context.Request.QueryString.Value + "&" + strQuery)
            : new QueryString("?" + strQuery);
    }

    private sealed class RoutingState
    {
        public HttpContext Context { get; init; }
        public IApi Api { get; init; }
        public IApplicationService Service { get; init; }
        public Config Config { get; init; }
        public string[] Segments { get; init; }
        public int Position { get; set; }
        public string Hostname { get; set; }
        public Site Site { get; set; }
        public Guid? LanguageId { get; set; }
        public PageBase Page { get; set; }
        public PageType PageType { get; set; }
        public PostBase Post { get; set; }
        public bool FoundCategory { get; set; }
        public bool FoundTag { get; set; }
        public StringBuilder Route { get; } = new();
        public StringBuilder Query { get; } = new();
    }

    /// <summary>
    /// Handles HTTP Caching Headers and checks if the client has the
    /// latest version in cache.
    /// </summary>
    /// <param name="context">The HTTP Cache</param>
    /// <param name="site">The current site</param>
    /// <param name="content">The current content</param>
    /// <param name="expires">How many minutes the cache should be valid</param>
    /// <returns>If the client has the latest version</returns>
    public bool HandleCache(HttpContext context, Site site, RoutedContentBase content, int expires)
    {
        var headers = context.Response.GetTypedHeaders();

        if (expires > 0 && content.Published.HasValue)
        {
            _logger?.LogDebug($"Setting HTTP Cache for [{ content.Slug }]");

            var lastModified = !site.ContentLastModified.HasValue || content.LastModified > site.ContentLastModified
                ? content.LastModified : site.ContentLastModified.Value;
            var etag = Utils.GenerateETag(content.Id.ToString(), lastModified);

            headers.CacheControl = new CacheControlHeaderValue
            {
                Public = true,
                MaxAge = TimeSpan.FromMinutes(expires),
            };
            headers.ETag = new EntityTagHeaderValue(etag);
            headers.LastModified = lastModified;

            if (HttpCaching.IsCached(context, etag, lastModified))
            {
                _logger?.LogInformation("Client has current version. Setting NotModified");
                context.Response.StatusCode = 304;

                return true;
            }
        }
        else
        {
            _logger?.LogDebug($"Setting HTTP NoCache for [{ content.Slug }]");

            headers.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true
            };
        }
        return false;
    }

    /// <summary>
    /// Gets the matching hostname.
    /// </summary>
    /// <param name="site">The site</param>
    /// <param name="hostname">The requested host</param>
    /// <returns>The hostname split into host and prefix</returns>
    private string[] GetMatchingHost(Site site, string hostname)
    {
        var result = new string[2];

        if (!string.IsNullOrEmpty(site.Hostnames))
        {
            foreach (var host in site.Hostnames.Split(","))
            {
                if (host.Trim().ToLower() == hostname)
                {
                    var segments = host.Split("/", StringSplitOptions.RemoveEmptyEntries);

                    result[0] = segments[0];
                    result[1] = segments.Length > 1 ? segments[1] : null;

                    break;
                }
            }
        }
        return result;
    }
}
