/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Piranha.AspNetCore.Security;
using Piranha.AspNetCore.Services;

namespace Piranha.AspNetCore.Http;

/// <summary>
/// The security middleware responsible for handling and
/// redirecting unauthorized content requests.
/// </summary>
public class SecurityMiddleware
{
    private readonly RequestDelegate _next;
    private readonly SecurityOptions _options;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="next">The next middleware component in the pipeline</param>
    /// <param name="options">The current routing options</param>
    public SecurityMiddleware(RequestDelegate next, IOptions<SecurityOptions> options)
    {
        _next = next;
        _options = options.Value;
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
            ctx.Response.Redirect($"{ _options.LoginUrl }?returnUrl={ service.Request.Url }");
        }
    }
}
