/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using Piranha.Manager.LocalAuth;

public static class SimpleSecurityExtensions
{
    /// <summary>
    /// Adds the simple security implementation to the service collection.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="users">The users to add for authentication</param>
    /// <returns>The updated collection</returns>
    public static IServiceCollection AddPiranhaSimpleSecurity(this IServiceCollection services, params Piranha.AspNetCore.SimpleUser[] users)
    {
        services.AddAuthentication("Piranha.SimpleSecurity")
            .AddCookie("Piranha.SimpleSecurity", o =>
            {
                o.LoginPath = new PathString("/manager/login");
                o.AccessDeniedPath = new PathString("/home/forbidden");
                o.ExpireTimeSpan = new TimeSpan(0, 30, 0);
            });
        return services.AddSingleton<ISecurity>(new Piranha.AspNetCore.SimpleSecurity(users));
    }

    /// <summary>
    /// Uses the piranha simple security implementation.
    /// </summary>
    /// <param name="builder">The current application builder</param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UsePiranhaSimpleSecurity(this IApplicationBuilder builder)
    {
        // Set logout url to point to local auth
        Piranha.App.Modules.Manager().LogoutUrl = "~/manager/logout";

        // Return the builder
        return builder.UseAuthentication();
    }
}