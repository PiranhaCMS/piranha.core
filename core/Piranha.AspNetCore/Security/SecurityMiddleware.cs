/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Piranha.AspNetCore.Services;

namespace Piranha.AspNetCore.Security
{
    /// <summary>
    /// The security middleware responsible for handling and
    /// redirecting unauthorized content requests.
    /// </summary>
    public class SecurityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly PiranhaRouteConfig _config;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="next">The next middleware component in the pipeline</param>
        /// <param name="config">The piranha route config</param>
        public SecurityMiddleware(RequestDelegate next, PiranhaRouteConfig config)
        {
            _next = next;
            _config = config;
        }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="ctx">The current http context</param>
        /// <param name="service">The piranha application service</param>
        /// <returns>An awaitable task</returns>
        public async Task InvokeAsync(HttpContext ctx, IApplicationService service)
        {
            // Execute the rest of the pipeline first
            await _next(ctx);

            // Check if we got back an unauthorized result
            // from the application
            if (ctx.Response.StatusCode == 401)
            {
                // Redirect to the configured login url
                ctx.Response.Redirect($"{ _config.LoginUrl }?returnUrl={ service.Request.Url }");
            }
        }
    }
}