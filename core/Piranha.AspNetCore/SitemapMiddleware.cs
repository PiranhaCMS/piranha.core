/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Piranha.AspNetCore.Services;
using X.Web.Sitemap;

namespace Piranha.AspNetCore
{
    public class SitemapMiddleware : MiddlewareBase
    {
        private readonly PiranhaRouteConfig _config;

        /// <summary>
        /// Creates a new middleware instance.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline</param>
        /// <param name="factory">The logger factory</param>
        /// <param name="config">The optional route config</param>
        public SitemapMiddleware(RequestDelegate next, ILoggerFactory factory = null, PiranhaRouteConfig config = null) : base(next, factory)
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
            var useSitemapRouting = _config != null ? _config.UseSitemapRouting : true;

            if (useSitemapRouting && !IsHandled(context) && !context.Request.Path.Value.StartsWith("/manager/assets/"))
            {
                var url = context.Request.Path.HasValue ? context.Request.Path.Value : "";
                var host = context.Request.Host.Host;
                var scheme = context.Request.Scheme;
                var port = context.Request.Host.Port;
                var baseUrl = scheme + "://" + host + (port.HasValue ? $":{port}" : "");

                if (url.ToLower() == "/sitemap.xml")
                {
                    _logger?.LogInformation($"Sitemap.xml requested, generating");

                    // Get the requested site by hostname
                    var siteId = service.Site.Id;

                    // Get the sitemap for the site
                    var pages = await api.Sites.GetSitemapAsync(siteId);

                    if (App.Hooks.OnGenerateSitemap != null)
                    {
                        // We need to clone the sitemap as it might be cached
                        // if we're going to modify it.
                        pages = App.Hooks.OnGenerateSitemap(Utils.DeepClone(pages));
                    }

                    // Generate sitemap.xml
                    var sitemap = new Sitemap();

                    foreach (var page in pages)
                    {
                        var urls = await GetPageUrlsAsync(api, page, baseUrl).ConfigureAwait(false);

                        if (urls.Count > 0)
                        {
                            sitemap.AddRange(urls);
                        }
                    }
                    context.Response.ContentType = "application/xml";
                    await context.Response.WriteAsync(sitemap.ToXml());
                    return;
                }
            }
            await _next.Invoke(context);
        }

        private async Task<List<Url>> GetPageUrlsAsync(IApi api, Piranha.Models.SitemapItem item, string baseUrl)
        {
            var urls = new List<Url>();

            if (item.Published.HasValue && item.Published.Value <= DateTime.Now)
            {
                urls.Add(new Url
                {
                    ChangeFrequency = ChangeFrequency.Daily,
                    Location = baseUrl + item.Permalink,
                    Priority = 0.5,
                    TimeStamp = item.LastModified
                });

                // Get all posts for the blog
                var posts = await api.Posts.GetAllAsync(item.Id);
                foreach (var post in posts)
                {
                    if (post.Published.HasValue && post.Published.Value <= DateTime.Now)
                    {
                        urls.Add(new Url
                        {
                            ChangeFrequency = ChangeFrequency.Daily,
                            Location = baseUrl + post.Permalink,
                            Priority = 0.5,
                            TimeStamp = post.LastModified
                        });
                    }
                }

                foreach (var child in item.Items)
                {
                    var childUrls = await GetPageUrlsAsync(api, child, baseUrl);

                    if (childUrls.Count > 0)
                    {
                        urls.AddRange(childUrls);
                    }
                }
            }
            return urls;
        }
    }
}
