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
using System.Threading.Tasks;

namespace Piranha.AspNetCore
{
    public class AliasMiddleware : MiddlewareBase
    {
        /// <summary>
        /// Creates a new middleware instance.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline</param>
        /// <param name="api">The current api</param>
        /// <param name="factory">The logger factory</param>
        public AliasMiddleware(RequestDelegate next, IApi api, ILoggerFactory factory = null) : base(next, api, factory) { }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">The current http context</param>
        /// <returns>An async task</returns>
        public override async Task Invoke(HttpContext context) {
            if (!IsHandled(context) && !context.Request.Path.Value.StartsWith("/manager/assets/")) {
                var url = context.Request.Path.HasValue ? context.Request.Path.Value : "";

                var response = AliasRouter.Invoke(api, url, context.Request.Host.Host);
                if (response != null) {
                    if (logger != null)
                        logger.LogInformation($"Found alias\n  Alias: {url}\n  Redirect: {response.RedirectUrl}");

                    context.Response.Redirect(response.RedirectUrl, response.RedirectType == Models.RedirectType.Permanent);
                    return;
                }
            }
            await next.Invoke(context);
        }
    }
}
