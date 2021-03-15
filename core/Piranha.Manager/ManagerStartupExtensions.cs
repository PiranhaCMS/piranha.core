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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.AspNetCore;

public static class ManagerStartupExtensions
{
    /// <summary>
    /// Uses the Piranha Manager services if simple startup is used.
    /// </summary>
    /// <param name="serviceBuilder">The service builder</param>
    /// <param name="jsonOptions">Optional JSON options</param>
    /// <returns>The updated builder</returns>
    public static PiranhaServiceBuilder UseManager(this PiranhaServiceBuilder serviceBuilder,
        Action<MvcNewtonsoftJsonOptions> jsonOptions = null)
    {
        // Add dependent services
        serviceBuilder.Services.AddLocalization(options =>
            options.ResourcesPath = "Resources"
        );
        serviceBuilder.Services.AddControllersWithViews();
        serviceBuilder.Services.AddRazorPages()
            .AddPiranhaManagerOptions(jsonOptions);

        // Add manager services
        serviceBuilder.Services.AddPiranhaManager();

        return serviceBuilder;
    }

    /// <summary>
    /// Uses the Piranha Manager if simple startup is enabled.
    /// </summary>
    /// <param name="applicationBuilder">The application builder</param>
    /// <returns>The updated builder</returns>
    public static PiranhaApplicationBuilder UseManager(this PiranhaApplicationBuilder applicationBuilder)
    {
        applicationBuilder.Builder.UsePiranhaManager();
        applicationBuilder.Builder.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            endpoints.MapPiranhaManager();
        });

        return applicationBuilder;
    }
}
