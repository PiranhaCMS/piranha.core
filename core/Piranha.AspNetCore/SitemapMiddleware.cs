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
using Piranha.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.Web.Sitemap;

namespace Piranha.AspNetCore
{
    public class SitemapMiddleware : MiddlewareBase
    {
        /// <summary>
        /// Creates a new middleware instance.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline</param>
        /// <param name="api">The current api</param>
        /// <param name="factory">The logger factory</param>
        public SitemapMiddleware(RequestDelegate next, IApi api, ILoggerFactory factory = null) : base(next, api, factory) { }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">The current http context</param>
        /// <returns>An async task</returns>
        public override async Task Invoke(HttpContext context) {
            if (!IsHandled(context) && !context.Request.Path.Value.StartsWith("/manager/assets/")) {
                var url = context.Request.Path.HasValue ? context.Request.Path.Value : "";
                var host = context.Request.Host.Host;
                var scheme = context.Request.Scheme;
                var port = context.Request.Host.Port;
                var baseUrl = scheme + "://" + host + (port.HasValue ? $":{port}" : "");

                if (url.ToLower() == "/sitemap.xml") {
                    if (logger != null)
                        logger.LogInformation($"Sitemap.xml requested, generating");
                    
                    // Get the requested site by hostname
                    var site = api.Sites.GetByHostname(host);
                    if (site == null)
                        site = api.Sites.GetDefault();

                    // Get the sitemap for the site
                    var pages = api.Sites.GetSitemap(site.Id);

                    // Generate sitemap.xml
                    var sitemap = new Sitemap();

                    foreach (var page in pages) {
                        var urls = GetPageUrls(page, baseUrl);

                        if (urls.Count > 0)
                            sitemap.AddRange(urls);
                    }
                    await context.Response.WriteAsync(sitemap.ToXml());
                }
            }
            await next.Invoke(context);
        }

        private List<Url> GetPageUrls(Models.SitemapItem item, string baseUrl) {
            var urls = new List<Url>();

            if (item.Published.HasValue) {
                urls.Add(new Url() {
                    ChangeFrequency = ChangeFrequency.Daily,
                    Location = baseUrl + item.Permalink,
                    Priority = 0.5,
                    TimeStamp = item.LastModified
                });

                // Check if this is a blog page
                var page = api.Pages.GetById(item.Id);
                if (page.ContentType == "Blog") {
                    // Get all posts for the blog
                    var posts = api.Posts.GetAll(page.Id);
                    foreach (var post in posts) {
                        urls.Add(new Url() {
                            ChangeFrequency = ChangeFrequency.Daily,
                            Location = baseUrl + post.Permalink,
                            Priority = 0.5,
                            TimeStamp = post.LastModified
                        });                        
                    }
                }

                foreach (var child in item.Items) {
                    var childUrls = GetPageUrls(child, baseUrl);

                    if (childUrls.Count > 0)
                        urls.AddRange(childUrls);
                }
            }
            return urls;
        }
    }
}
