/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.WebApi;

public static class WebApiModuleExtensions
{
    /// <summary>
    /// Adds the Piranha Api module.
    /// </summary>
    /// <param name="serviceBuilder">The service builder</param>
    /// <param name="configure">The optional api configuration</param>
    /// <returns>The services</returns>
    public static PiranhaServiceBuilder UseApi(this PiranhaServiceBuilder serviceBuilder,
        Action<WebApiOptions> configure = null)
    {
        serviceBuilder.Services.AddPiranhaApi(configure);

        return serviceBuilder;
    }

    /// <summary>
    /// Adds the Piranha Api module.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="configure">The optional api configuration</param>
    /// <returns>The services</returns>
    public static IServiceCollection AddPiranhaApi(this IServiceCollection services,
        Action<WebApiOptions> configure = null)
    {
        // Configure the api module
        var options = new WebApiOptions();
        configure?.Invoke(options);
        Module.AllowAnonymousAccess = options.AllowAnonymousAccess;

        // Add the api module
        Piranha.App.Modules.Register<Piranha.WebApi.Module>();

        // Setup authorization policies
        services.AddAuthorization(o => {
            // Media policies
            o.AddPolicy(Permissions.Media, policy => {
                policy.RequireClaim(Permissions.Media, Permissions.Media);
            });

            // Page policies
            o.AddPolicy(Permissions.Pages, policy => {
                policy.RequireClaim(Permissions.Pages, Permissions.Pages);
            });

            // Posts policies
            o.AddPolicy(Permissions.Posts, policy => {
                policy.RequireClaim(Permissions.Posts, Permissions.Posts);
            });

            // Sitemap policies
            o.AddPolicy(Permissions.Sitemap, policy => {
                policy.RequireClaim(Permissions.Sitemap, Permissions.Sitemap);
            });
        });

        // Return the service collection
        return services;
    }
}
