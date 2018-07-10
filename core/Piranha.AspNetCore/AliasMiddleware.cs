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
using System;
using System.Threading.Tasks;
using Piranha.AspNetCore.Services;
using Piranha.Web;

namespace Piranha.AspNetCore
{
    public class AliasMiddleware : MiddlewareBase
    {
        /// <summary>
        /// Creates a new middleware instance.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline</param>
        /// <param name="factory">The logger factory</param>
        public AliasMiddleware(RequestDelegate next, ILoggerFactory factory = null) : base(next, factory) { }

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

                var response = AliasRouter.Invoke(api, url, service.Site.Id);
                if (response != null)
                {
                    if (_logger != null)
                        _logger.LogInformation($"Found alias\n  Alias: {url}\n  Redirect: {response.RedirectUrl}");

                    context.Response.Redirect(response.RedirectUrl, response.RedirectType == Models.RedirectType.Permanent);
                    return;
                }
            }
            await _next.Invoke(context);
        }
    }
}
