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
using Piranha.Security;

/// <summary>
/// Extensions methods for setting up Piranha in Configure
/// and ConfigureServices.
/// </summary>
public static class PiranhaStartupExtensions
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
            LoginUrl = serviceBuilder.LoginUrl,
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
        services
            .AddPiranha()
            .AddScoped<Piranha.AspNetCore.Services.IApplicationService, Piranha.AspNetCore.Services.ApplicationService>()
            .AddScoped<Piranha.AspNetCore.Services.IModelLoader, Piranha.AspNetCore.Services.ModelLoader>()
            .AddAuthorization(o =>
            {
                o.AddPolicy(Permission.PagePreview, policy =>
                {
                    policy.RequireClaim(Permission.PagePreview, Permission.PagePreview);
                });
                o.AddPolicy(Permission.PostPreview, policy =>
                {
                    policy.RequireClaim(Permission.PostPreview, Permission.PostPreview);
                });
            });
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
            .UseSecurityMiddleware()
            .UseStaticFiles()
            .UseMiddleware<Piranha.AspNetCore.PiranhaMiddleware>()
            .UseMiddleware<Piranha.AspNetCore.SitemapMiddleware>()
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization();

        options?.Invoke(piranhaOptions);

        return piranhaOptions.Builder;
    }
}
