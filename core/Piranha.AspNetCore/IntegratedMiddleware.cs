/*
 * Copyright (c) 2019 Håkan Edling
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
using Piranha.AspNetCore.Services;
using Piranha.Models;

namespace Piranha.AspNetCore
{
    public class IntegratedMiddleware : MiddlewareBase
    {
        /// <summary>
        /// Creates a new middleware instance.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline</param>
        /// <param name="factory">The logger factory</param>
        public IntegratedMiddleware(RequestDelegate next, ILoggerFactory factory = null) : base(next, factory) { }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">The current http context</param>
        /// <param name="api">The current api</param>
        /// <returns>An async task</returns>
        public override async Task Invoke(HttpContext context, IApi api, IApplicationService service)
        {
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

                // Try to get the requested site by hostname & prefix
                if (segments.Length > 0)
                {
                    site = await api.Sites.GetByHostnameAsync($"{context.Request.Host.Host}/{segments[0]}")
                        .ConfigureAwait(false);

                    if (site != null)
                    {
                        context.Request.Path = "/" + string.Join("/", segments.Skip(1));
                        pos = 1;
                    }
                }

                // Try to get the requested site by hostname
                if (site == null)
                {
                    site = await api.Sites.GetByHostnameAsync(context.Request.Host.Host)
                        .ConfigureAwait(false);
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

                //
                // 3: Get the current page
                //
                PageBase page = null;
                PageType pageType = null;

                if (segments != null && segments.Length > pos)
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
                    service.CurrentPage = page;
                }

                //
                // 4: Get the current post
                //
                PostBase post = null;
                PostType postType = null;

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
                    postType = App.PostTypes.GetById(post.TypeId);
                    service.CurrentPost = post;
                }

#if DEBUG
                _logger?.LogDebug($"FOUND SITE: [{ site.Id }]");
                if (page != null)
                {
                    _logger?.LogDebug($"FOUND PAGE: [{ page.Id }]");
                }

                if (post != null)
                {
                    _logger?.LogDebug($"FOUND POST: [{ post.Id }]");
                }
#endif

                //
                // 5: Route request
                //
                var route = new StringBuilder();
                var query = new StringBuilder();

                if (post != null)
                {
                    route.Append(post.Route ?? "/post");
                    for (var n = pos; n < segments.Length; n++)
                    {
                        route.Append("/");
                        route.Append(segments[n]);
                    }

                    query.Append("?id=");
                    query.Append(post.Id);
                }
                else if (page != null)
                {
                    route.Append(page.Route ?? (pageType.IsArchive ? "/archive" : "/page"));
                    for (var n = pos; n < segments.Length; n++)
                    {
                        route.Append("/");
                        route.Append(segments[n]);
                    }

                    query.Append("?id=");
                    query.Append(page.Id);

                    if (!page.ParentId.HasValue && page.SortOrder == 0)
                    {
                        query.Append("&startpage=true");
                    }
                }

                if (route.Length > 0)
                {
                    var strRoute = route.ToString();
                    var strQuery = query.ToString();

#if DEBUG
                    _logger?.LogDebug($"SETTING ROUTE: [{ strRoute }]");
                    _logger?.LogDebug($"SETTING QUERY: [{ strQuery }]");
#endif

                    context.Request.Path = new PathString(strRoute);
                    context.Request.QueryString = new QueryString(strQuery);
                }
            }
            await _next.Invoke(context);
        }
    }
}
