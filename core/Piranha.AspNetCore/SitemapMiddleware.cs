/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Piranha.AspNetCore.Services;
using Piranha.Web;
using X.Web.Sitemap;

namespace Piranha.AspNetCore
{
    public class SitemapMiddleware : MiddlewareBase
    {
        /// <summary>
        /// Creates a new middleware instance.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline</param>
        /// <param name="factory">The logger factory</param>
        public SitemapMiddleware(RequestDelegate next, ILoggerFactory factory = null) : base(next, factory) { }

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
                var host = context.Request.Host.Host;
                var scheme = context.Request.Scheme;
                var port = context.Request.Host.Port;
                var baseUrl = scheme + "://" + host + (port.HasValue ? $":{port}" : "");

                if (url.ToLower() == "/sitemap.xml")
                {
                    if (_logger != null)
                        _logger.LogInformation($"Sitemap.xml requested, generating");

                    // Get the requested site by hostname
                    var siteId = service.Site.Id;

                    // Get the sitemap for the site
                    var pages = api.Sites.GetSitemap(siteId);

                    // Generate sitemap.xml
                    var sitemap = new Sitemap();

                    foreach (var page in pages)
                    {
                        var urls = GetPageUrls(api, page, baseUrl);

                        if (urls.Count > 0)
                            sitemap.AddRange(urls);
                    }
                    context.Response.ContentType = "application/xml";
                    await context.Response.WriteAsync(sitemap.ToXml());
                    return;
                }
            }
            await _next.Invoke(context);
        }

        private List<Url> GetPageUrls(IApi api, Models.SitemapItem item, string baseUrl)
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
                var posts = api.Posts.GetAll(item.Id);
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
                    var childUrls = GetPageUrls(api, child, baseUrl);

                    if (childUrls.Count > 0)
                        urls.AddRange(childUrls);
                }
            }
            return urls;
        }
    }
}
