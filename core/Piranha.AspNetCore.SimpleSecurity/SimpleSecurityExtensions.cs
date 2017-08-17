/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;

public static class SimpleSecurityExtensions
{
    /// <summary>
    /// Adds the simple security implementation to the service collection.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <returns>The updated collection</returns>
    public static IServiceCollection AddPiranhaSimpleSecurity(this IServiceCollection services, params Piranha.AspNetCore.SimpleUser[] users) {
        return services.AddSingleton<Piranha.ISecurity>(new Piranha.AspNetCore.SimpleSecurity(users));
    }

    /// <summary>
    /// Uses the piranha simple security implementation.
    /// </summary>
    /// <param name="builder">The current application builder</param>
    /// <returns>The builder</returns>    
    public static IApplicationBuilder UsePiranhaSimpleSecurity(this IApplicationBuilder builder) {
        return builder
            .UseCookieAuthentication(new CookieAuthenticationOptions {
                AuthenticationScheme = "Piranha.SimpleSecurity",
                LoginPath = "/manager/login",
                AccessDeniedPath = "/home/forbidden",
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            });            
    }
}