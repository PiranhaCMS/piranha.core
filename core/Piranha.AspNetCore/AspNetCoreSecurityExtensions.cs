/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.AspNetCore.Security;

/// <summary>
/// Security extensions for simplifying authorization in
/// the client application.
/// </summary>
public static class AspNetCoreSecurityExtensions
{
    /// <summary>
    /// Adds authorization with the given application policies to the aplication.
    /// </summary>
    /// <param name="builder">The service builder</param>
    /// <param name="options">The security options</param>
    /// <returns>The service builder</returns>
    public static PiranhaServiceBuilder UseSecurity(this PiranhaServiceBuilder builder, Action<SecurityBuilder> options)
    {
        // Add authentication
        builder.Services.AddAuthorization(o =>
        {
            // Invoke the security options
            var securityBuilder = new SecurityBuilder(o, builder);
            options?.Invoke(securityBuilder);
        });
        return builder;
    }

    /// <summary>
    /// Adds the security middleware to the pipeline.
    /// </summary>
    /// <param name="builder">The current application builder</param>
    /// <returns>The update builder</returns>
    public static IApplicationBuilder UseSecurityMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<Piranha.AspNetCore.Security.SecurityMiddleware>();
    }
}