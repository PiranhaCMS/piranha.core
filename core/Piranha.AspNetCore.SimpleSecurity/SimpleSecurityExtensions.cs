/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.AspNetCore;

public static class SimpleSecurityExtensions
{
    /// <summary>
    /// Adds the simple security implementation to the service collection.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <returns>The updated collection</returns>
    public static IServiceCollection AddPiranhaSimpleSecurity(this IServiceCollection services, params SimpleUser[] users)
    {
        services.AddAuthentication("Piranha.SimpleSecurity")
            .AddCookie("Piranha.SimpleSecurity", o =>
            {
                o.LoginPath = new PathString("/manager/login");
                o.AccessDeniedPath = new PathString("/home/forbidden");
                o.ExpireTimeSpan = new TimeSpan(0, 30, 0);
            });
        return services.AddSingleton<ISecurity>(new SimpleSecurity(users));
    }

    /// <summary>
    /// Uses the piranha simple security implementation.
    /// </summary>
    /// <param name="builder">The current application builder</param>
    /// <returns>The builder</returns>    
    public static IApplicationBuilder UsePiranhaSimpleSecurity(this IApplicationBuilder builder)
    {
        return builder.UseAuthentication();
    }
}