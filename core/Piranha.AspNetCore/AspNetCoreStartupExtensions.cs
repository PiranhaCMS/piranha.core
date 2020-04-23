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
using Piranha.AspNetCore;

public static class AspNetCoreStartupExtensions
{
    /// <summary>
    /// Adds the core Piranha services if simple startup is used.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="options">The builder options</param>
    /// <returns>The updated service collection</returns>
    public static IServiceCollection AddPiranha(this IServiceCollection services, Action<PiranhaServiceBuilder> options)
    {
        var serviceBuilder = new PiranhaServiceBuilder(services);

        options?.Invoke(serviceBuilder);

        var config = new PiranhaRouteConfig
        {
            UseAliasRouting = serviceBuilder.UseAliasRouting,
            UseArchiveRouting = serviceBuilder.UseArchiveRouting,
            UsePageRouting = serviceBuilder.UsePageRouting,
            UsePostRouting = serviceBuilder.UsePostRouting,
            UseSiteRouting = serviceBuilder.UseSiteRouting,
            UseSitemapRouting = serviceBuilder.UseSitemapRouting,
            UseStartpageRouting = serviceBuilder.UseStartpageRouting
        };
        services.AddSingleton<PiranhaRouteConfig>(config);

        services.AddControllersWithViews();
        var mvcBuilder = services.AddRazorPages();
        if (serviceBuilder.AddRazorRuntimeCompilation)
        {
            mvcBuilder.AddRazorRuntimeCompilation();
        }
        services.AddPiranha();
        services.AddPiranhaApplication();

        return serviceBuilder.Services;
    }

    /// <summary>
    /// Simple startup with integrated middleware that also adds common
    /// dependencies needed for an integrated web application.
    /// </summary>
    /// <param name="builder">The application builder</param>
    /// <param name="options">Action for configuring the builder</param>
    /// <returns>The updated application builder</returns>
    public static IApplicationBuilder UsePiranha(this IApplicationBuilder builder, Action<PiranhaApplicationBuilder> options)
    {
        var piranhaOptions = new PiranhaApplicationBuilder(builder);

        piranhaOptions.Builder
            .UseStaticFiles()
            .UseIntegratedPiranha()
            .UsePiranhaSitemap()
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization();

        options?.Invoke(piranhaOptions);

        return piranhaOptions.Builder;
    }
}
