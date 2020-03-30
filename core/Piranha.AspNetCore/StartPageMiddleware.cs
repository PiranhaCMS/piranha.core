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
using Microsoft.Net.Http.Headers;
using System;
using System.Threading.Tasks;
using Piranha.AspNetCore.Services;
using Piranha.Web;

namespace Piranha.AspNetCore
{
    public class StartPageMiddleware : MiddlewareBase
    {
        /// <summary>
        /// Creates a new middleware instance.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline</param>
        /// <param name="factory">The logger factory</param>
        public StartPageMiddleware(RequestDelegate next, ILoggerFactory factory = null) : base(next, factory) { }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">The current http context</param>
        /// <param name="api">The current api</param>
        /// <param name="service">The application service</param>
        /// <returns>An async task</returns>
        public override async Task Invoke(HttpContext context, IApi api, IApplicationService service)
        {
            if (!IsHandled(context) && !context.Request.Path.Value.StartsWith("/manager/assets/"))
            {
                var url = context.Request.Path.HasValue ? context.Request.Path.Value : "";
                var siteId = service.Site.Id;
                var authorized = true;

                var response = await StartPageRouter.InvokeAsync(api, url, siteId);
                if (response != null)
                {
                    _logger?.LogInformation($"Found startpage\n  Route: {response.Route}\n  Params: {response.QueryString}");

                    if (!response.IsPublished)
                    {
                        if (!context.User.HasClaim(Security.Permission.PagePreview, Security.Permission.PagePreview))
                        {
                            _logger?.LogInformation($"User not authorized to preview unpublished page");
                            authorized = false;
                        }
                    }

                    if (authorized)
                    {
                        service.PageId = response.PageId;

                        using (var config = new Config(api))
                        {
                            var headers = context.Response.GetTypedHeaders();

                            if (config.CacheExpiresPages > 0)
                            {
                                _logger?.LogInformation("Caching enabled. Setting MaxAge, LastModified & ETag");

                                headers.CacheControl = new CacheControlHeaderValue
                                {
                                    Public = true,
                                    MaxAge = TimeSpan.FromMinutes(config.CacheExpiresPages),
                                };

                                headers.ETag = new EntityTagHeaderValue(response.CacheInfo.EntityTag);
                                headers.LastModified = response.CacheInfo.LastModified;
                            }
                            else
                            {
                                headers.CacheControl = new CacheControlHeaderValue
                                {
                                    NoCache = true
                                };
                            }
                        }

                        if (HttpCaching.IsCached(context, response.CacheInfo))
                        {
                            _logger?.LogInformation("Client has current version. Returning NotModified");

                            context.Response.StatusCode = 304;
                            return;
                        }
                        else
                        {
                            context.Request.Path = new PathString(response.Route);

                            if (context.Request.QueryString.HasValue)
                            {
                                context.Request.QueryString = new QueryString(context.Request.QueryString.Value + "&" + response.QueryString);
                            }
                            else
                            {
                                context.Request.QueryString = new QueryString("?" + response.QueryString);
                            }
                        }
                    }
                }
            }
            await _next.Invoke(context);
        }
    }
}
