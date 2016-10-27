/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Builder;

public static class ManagerModuleExtensions
{
    /// <summary>
    /// Adds the Piranha manager module.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <returns>The services</returns>
    public static IServiceCollection AddPiranhaManager(this IServiceCollection services) {
        var assembly = typeof(ManagerModuleExtensions).GetTypeInfo().Assembly;
        var provider = new EmbeddedFileProvider(assembly);

        // Add the file provider to the Razor view engine
        services.Configure<RazorViewEngineOptions>(options => {
            options.FileProviders.Add(provider);
        });

        // Add the manager module
        Piranha.App.Modules.Add(new Piranha.Manager.Module());

        // Return the service collection
        return services;
    }

    /// <summary>
    /// Uses the piranha middleware.
    /// </summary>
    /// <param name="builder">The current application builder</param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UsePiranhaManager(this IApplicationBuilder builder) {
        return builder
            .UseMiddleware<Piranha.Manager.ResourceMiddleware>();
    }
}
