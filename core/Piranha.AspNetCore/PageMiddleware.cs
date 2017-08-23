/*
 * Copyright (c) 2016-2017 Håkan Edling
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
using System.Threading.Tasks;

namespace Piranha.AspNetCore
{
    public class PageMiddleware : MiddlewareBase
    {
        /// <summary>
        /// Creates a new middleware instance.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline</param>
        /// <param name="api">The current api</param>
        /// <param name="factory">The logger factory</param>
        public PageMiddleware(RequestDelegate next, IApi api, ILoggerFactory factory = null) : base(next, api, factory) { }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">The current http context</param>
        /// <returns>An async task</returns>
        public override async Task Invoke(HttpContext context) {
            if (!IsHandled(context) && !context.Request.Path.Value.StartsWith("/manager/assets/")) {
                var url = context.Request.Path.HasValue ? context.Request.Path.Value : "";

                var response = PageRouter.Invoke(api, url);
                if (response != null) {
                    if (logger != null)
                        logger.LogInformation($"Found page\n  Route: {response.Route}\n  Params: {response.QueryString}");

                    if (string.IsNullOrWhiteSpace(response.RedirectUrl)) {
                        using (var config = new Config(api)) {
                            var headers = context.Response.GetTypedHeaders();

                            if (config.CacheExpiresPages > 0) {
                                if (logger != null)
                                    logger.LogInformation("Caching enabled. Setting MaxAge, LastModified & ETag");

                                headers.CacheControl = new CacheControlHeaderValue() {
                                    Public = true,
                                    MaxAge = TimeSpan.FromMinutes(config.CacheExpiresPages),
                                };

                                headers.Headers["ETag"] = response.CacheInfo.EntityTag;
                                headers.LastModified = response.CacheInfo.LastModified;
                            } else {
                                headers.CacheControl = new CacheControlHeaderValue() {
                                    NoCache = true
                                };
                            }
                        }

                        if (HttpCaching.IsCached(context, response.CacheInfo)) {
                            if (logger != null)
                                logger.LogInformation("Client has current version. Returning NotModified");

                            context.Response.StatusCode = 304;
                            return;
                        } else {
                            context.Request.Path = new PathString(response.Route);

                            if (context.Request.QueryString.HasValue) {
                                context.Request.QueryString = new QueryString(context.Request.QueryString.Value + "&" + response.QueryString);
                            } else context.Request.QueryString = new QueryString("?" + response.QueryString);
                        }
                    } else {
                        if (logger != null)
                            logger.LogInformation($"Redirecting to url: {response.RedirectUrl}");

                        context.Response.Redirect(response.RedirectUrl, response.RedirectType == Models.RedirectType.Permanent);
                        return;
                    }
                }
            }
            await next.Invoke(context);
        }
    }
}
