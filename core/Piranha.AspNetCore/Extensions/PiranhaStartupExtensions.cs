/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.AspNetCore;
using Piranha.AspNetCore.Hosting;
using Piranha.Security;

/// <summary>
/// Extensions methods for setting up Piranha in Configure
/// and ConfigureServices.
/// </summary>
public static class PiranhaStartupExtensions
{
    /// <summary>
    /// Adds the services needed to run a Piranha client application.
    /// </summary>
    /// <param name="serviceBuilder">The current service builder</param>
    /// <param name="options">The optional routing options</param>
    /// <returns>The updated service builder</returns>
    public static PiranhaServiceBuilder UseCms(this PiranhaServiceBuilder serviceBuilder,
        Action<RoutingOptions> options = null)
    {
        serviceBuilder.Services.AddControllersWithViews();
        var mvcBuilder = serviceBuilder.Services.AddRazorPages();
        if (serviceBuilder.AddRazorRuntimeCompilation)
        {
            mvcBuilder.AddRazorRuntimeCompilation();
        }
        serviceBuilder.Services
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

        serviceBuilder.Services.AddTransient<IStartupFilter, PiranhaStartupFilter>();
        serviceBuilder.Services.Configure<RoutingOptions>(o => options?.Invoke(o));

        return serviceBuilder;
    }
}
