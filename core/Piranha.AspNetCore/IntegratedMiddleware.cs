/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Piranha.AspNetCore.Services;
using Piranha.Models;

namespace Piranha.AspNetCore
{
    public class IntegratedMiddleware : MiddlewareBase
    {
        private readonly PiranhaRouteConfig _config;

        /// <summary>
        /// Creates a new middleware instance.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline</param>
        /// <param name="config">The current route configuration</param>
        /// <param name="factory">The logger factory</param>
        public IntegratedMiddleware(RequestDelegate next, PiranhaRouteConfig config, ILoggerFactory factory = null) : base(next, factory)
        {
            _config = config;
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
            var appConfig = new Config(api);

            if (!IsHandled(context) && !context.Request.Path.Value.StartsWith("/manager/assets/"))
            {
                var url = context.Request.Path.HasValue ? context.Request.Path.Value : "";
                var segments = url.Substring(1).Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                int pos = 0;

                //
                // 1: Store raw url
                //
                service.Url = context.Request.Path.Value;

                //
                // 2: Get the current site
                //
                Site site = null;

                var hostname = context.Request.Host.Host;

                if (_config.UseSiteRouting)
                {
                    // Try to get the requested site by hostname & prefix
                    if (segments.Length > 0)
                    {
                        var prefixedHostname = $"{hostname}/{segments[0]}";
                        site = await api.Sites.GetByHostnameAsync(prefixedHostname)
                            .ConfigureAwait(false);

                        if (site != null)
                        {
                            context.Request.Path = "/" + string.Join("/", segments.Skip(1));
                            hostname = prefixedHostname;
                            pos = 1;
                        }
                    }

                    // Try to get the requested site by hostname
                    if (site == null)
                    {
                        site = await api.Sites.GetByHostnameAsync(context.Request.Host.Host)
                            .ConfigureAwait(false);
                    }
                }

                // If we didn't find the site, get the default site
                if (site == null)
                {
                    site = await api.Sites.GetDefaultAsync()
                        .ConfigureAwait(false);
                }

                if (site != null)
                {
                    // Update application service
                    service.Site.Id = site.Id;
                    service.Site.Culture = site.Culture;
                    service.Site.Sitemap = await api.Sites.GetSitemapAsync(site.Id);

                    // Set current culture if specified in site
                    if (!string.IsNullOrEmpty(site.Culture))
                    {
                        var cultureInfo = new CultureInfo(service.Site.Culture);
                        CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = cultureInfo;
                    }
                }
                else
                {
                    // There's no sites available, let the application finish
                    await _next.Invoke(context);
                    return;
                }

                // Store hostname
                service.Hostname = hostname;

                //
                // Check if we shouldn't handle empty requests for start page
                //
                if (segments.Length == 0 && !_config.UseStartpageRouting)
                {
                    await _next.Invoke(context);
                    return;
                }

                //
                // 3: Check for alias
                //
                if (_config.UseAliasRouting && segments.Length > pos)
                {
                    var alias = await api.Aliases.GetByAliasUrlAsync($"/{ string.Join("/", segments.Subset(pos)) }", service.Site.Id);

                    if (alias != null)
                    {
                        context.Response.Redirect(alias.RedirectUrl, alias.Type == RedirectType.Permanent);
                        return;
                    }
                }

                //
                // 4: Get the current page
                //
                PageBase page = null;
                PageType pageType = null;

                if (segments.Length > pos)
                {
                    // Scan for the most unique slug
                    for (var n = segments.Length; n > pos; n--)
                    {
                        var slug = string.Join("/", segments.Subset(pos, n));
                        page = await api.Pages.GetBySlugAsync<PageBase>(slug, site.Id)
                            .ConfigureAwait(false);

                        if (page != null)
                        {
                            pos = pos + n;
                            break;
                        }
                    }
                }
                else
                {
                    page = await api.Pages.GetStartpageAsync<PageBase>(site.Id)
                        .ConfigureAwait(false);
                }

                if (page != null)
                {
                    pageType = App.PageTypes.GetById(page.TypeId);
                    service.PageId = page.Id;

                    // Only cache published pages
                    if (page.IsPublished)
                    {
                        service.CurrentPage = page;
                    }
                }

                //
                // 5: Get the current post
                //
                PostBase post = null;

                if (_config.UsePostRouting)
                {
                    if (page != null && pageType.IsArchive && segments.Length > pos)
                    {
                        post = await api.Posts.GetBySlugAsync<PostBase>(page.Id, segments[pos])
                            .ConfigureAwait(false);

                        if (post != null)
                        {
                            pos++;
                        }
                    }

                    if (post != null)
                    {
                        App.PostTypes.GetById(post.TypeId);

                        // Onlyc cache published posts
                        if (post.IsPublished)
                        {
                            service.CurrentPost = post;
                        }
                    }
                }

                _logger?.LogDebug($"Found Site: [{ site.Id }]");
                if (page != null)
                {
                    _logger?.LogDebug($"Found Page: [{ page.Id }]");
                }

                if (post != null)
                {
                    _logger?.LogDebug($"Found Post: [{ post.Id }]");
                }

                //
                // 6: Route request
                //
                var route = new StringBuilder();
                var query = new StringBuilder();

                if (post != null)
                {
                    if (string.IsNullOrWhiteSpace(post.RedirectUrl))
                    {
                        // Handle HTTP caching
                        if (HandleCache(context, site, post, appConfig.CacheExpiresPosts))
                        {
                            // Client has latest version
                            return;
                        }

                        route.Append(post.Route ?? "/post");
                        for (var n = pos; n < segments.Length; n++)
                        {
                            route.Append("/");
                            route.Append(segments[n]);
                        }

                        query.Append("id=");
                        query.Append(post.Id);
                    }
                    else
                    {
                        _logger?.LogDebug($"Setting redirect: [{ post.RedirectUrl }]");

                        context.Response.Redirect(post.RedirectUrl, post.RedirectType == RedirectType.Permanent);
                        return;
                    }
                }
                else if (page != null && _config.UsePageRouting)
                {
                    if (string.IsNullOrWhiteSpace(page.RedirectUrl))
                    {
                        route.Append(page.Route ?? (pageType.IsArchive ? "/archive" : "/page"));

                        // Set the basic query
                        query.Append("id=");
                        query.Append(page.Id);

                        if (!page.ParentId.HasValue && page.SortOrder == 0)
                        {
                            query.Append("&startpage=true");
                        }

                        if (!pageType.IsArchive || !_config.UseArchiveRouting)
                        {
                            if (HandleCache(context, site, page, appConfig.CacheExpiresPages))
                            {
                                // Client has latest version.
                                return;
                            }

                            // This is a regular page, append trailing segments
                            for (var n = pos; n < segments.Length; n++)
                            {
                                route.Append("/");
                                route.Append(segments[n]);
                            }
                        }
                        else if (post == null)
                        {
                            // This is an archive, check for archive params
                            int? year = null;
                            bool foundCategory = false;
                            bool foundTag = false;
                            bool foundPage = false;

                            for (var n = pos; n < segments.Length; n++)
                            {
                                if (segments[n] == "category" && !foundPage)
                                {
                                    foundCategory = true;
                                    continue;
                                }

                                if (segments[n] == "tag" && !foundPage && !foundCategory)
                                {
                                    foundTag = true;
                                    continue;
                                }

                                if (segments[n] == "page")
                                {
                                    foundPage = true;
                                    continue;
                                }

                                if (foundCategory)
                                {
                                    try
                                    {
                                        var categoryId = (await api.Posts.GetCategoryBySlugAsync(page.Id, segments[n]).ConfigureAwait(false))?.Id;

                                        if (categoryId.HasValue)
                                        {
                                            query.Append("&category=");
                                            query.Append(categoryId);
                                        }
                                    }
                                    finally
                                    {
                                        foundCategory = false;
                                    }
                                }

                                if (foundTag)
                                {
                                    try
                                    {
                                        var tagId = (await api.Posts.GetTagBySlugAsync(page.Id, segments[n]).ConfigureAwait(false))?.Id;

                                        if (tagId.HasValue)
                                        {
                                            query.Append("&tag=");
                                            query.Append(tagId);
                                        }
                                    }
                                    finally
                                    {
                                        foundTag = false;
                                    }
                                }

                                if (foundPage)
                                {
                                    try
                                    {
                                        var pageNum = Convert.ToInt32(segments[n]);
                                        query.Append("&page=");
                                        query.Append(pageNum);
                                        query.Append("&pagenum=");
                                        query.Append(pageNum);
                                    }
                                    catch
                                    {
                                        // We don't care about the exception, we just
                                        // discard malformed input
                                    }
                                    // Page number should always be last, break the loop
                                    break;
                                }

                                if (!year.HasValue)
                                {
                                    try
                                    {
                                        year = Convert.ToInt32(segments[n]);

                                        if (year.Value > DateTime.Now.Year)
                                        {
                                            year = DateTime.Now.Year;
                                        }
                                        query.Append("&year=");
                                        query.Append(year);
                                    }
                                    catch
                                    {
                                        // We don't care about the exception, we just
                                        // discard malformed input
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        var month = Math.Max(Math.Min(Convert.ToInt32(segments[n]), 12), 1);
                                        query.Append("&month=");
                                        query.Append(month);
                                    }
                                    catch
                                    {
                                        // We don't care about the exception, we just
                                        // discard malformed input
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        _logger?.LogDebug($"Setting redirect: [{ page.RedirectUrl }]");

                        context.Response.Redirect(page.RedirectUrl, page.RedirectType == RedirectType.Permanent);
                        return;
                    }
                }

                if (route.Length > 0)
                {
                    var strRoute = route.ToString();
                    var strQuery = query.ToString();

                    _logger?.LogDebug($"Setting Route: [{ strRoute }]");
                    _logger?.LogDebug($"Setting Query: [{ strQuery }]");

                    context.Request.Path = new PathString(strRoute);
                    if (context.Request.QueryString.HasValue)
                    {
                        context.Request.QueryString =
                            new QueryString(context.Request.QueryString.Value + "&" + strQuery);
                    }
                    else {
                        context.Request.QueryString =
                            new QueryString("?" + strQuery);
                    }
                }
            }
            await _next.Invoke(context);
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
    }
}
