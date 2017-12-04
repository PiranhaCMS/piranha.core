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
using Piranha.Web;
using System;
using System.Threading.Tasks;

namespace Piranha.AspNetCore
{
    public class ArchiveMiddleware : MiddlewareBase
    {
        /// <summary>
        /// Creates a new middleware instance.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline</param>
        public ArchiveMiddleware(RequestDelegate next, IApi api, ILoggerFactory factory = null) : base(next, api, factory) { }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">The current http context</param>
        /// <returns>An async task</returns>
        public override async Task Invoke(HttpContext context) {
            if (!IsHandled(context) && !context.Request.Path.Value.StartsWith("/manager/assets/")) {
                var url = context.Request.Path.HasValue ? context.Request.Path.Value : "";

                var response = ArchiveRouter.Invoke(api, url);
                if (response != null) {
                    if (logger != null)
                        logger.LogInformation($"Found archive\n  Route: {response.Route}\n  Params: {response.QueryString}");

                    context.Request.Path = new PathString(response.Route);

                    if (context.Request.QueryString.HasValue) {
                        context.Request.QueryString = new QueryString(context.Request.QueryString.Value + "&" + response.QueryString);
                    } else context.Request.QueryString = new QueryString("?" + response.QueryString);
                }
            }
            await next.Invoke(context);
        }
    }
}
