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
    public class SecurityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly PiranhaRouteConfig _config;

        public SecurityMiddleware(RequestDelegate next, PiranhaRouteConfig config)
        {
            _next = next;
            _config = config;
        }

        public async Task InvokeAsync(HttpContext ctx, IApplicationService service)
        {
            await _next(ctx);

            if (ctx.Response.StatusCode == 401)
            {
                ctx.Response.Redirect($"{ _config.LoginUrl }?returnUrl={ service.Request.Url }");
            }
        }
    }
}