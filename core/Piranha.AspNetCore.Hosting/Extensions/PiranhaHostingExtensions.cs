/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.AspNetCore;

/// <summary>
/// Extensions methods for hosting Piranha in an ASP.NET
/// application.
/// </summary>
public static class PiranhaHostingExtensions
{
    /// <summary>
    /// Adds Piranha to the web application builder.
    /// </summary>
    /// <param name="builder">The web application builder</param>
    /// <param name="options">The startup options</param>
    /// <returns>The WebApplicationBuilder</returns>
    public static WebApplicationBuilder AddPiranha(this WebApplicationBuilder builder, Action<PiranhaServiceBuilder> options)
    {
        builder.Services.AddPiranha(options);
        return builder;
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
        var applicationBuilder = new PiranhaApplicationBuilder(builder);

        applicationBuilder.Builder
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization();

        options?.Invoke(applicationBuilder);

        // Configure all registered endpoints
        applicationBuilder.Builder.UseEndpoints(endpoints =>
        {
            foreach (var endpoint in applicationBuilder.Endpoints)
            {
                endpoint.Invoke(endpoints);
            }
        });
        return applicationBuilder.Builder;
    }

    /// <summary>
    /// Uses the Piranha runtime components for ASP.NET and sets up
    /// common dependencies needed for an integrated web application.
    /// </summary>
    /// <param name="app">The web application</param>
    /// <param name="options">Action for configuring the application</param>
    /// <returns>The web application</returns>
    public static WebApplication UsePiranha(this WebApplication app, Action<PiranhaApplication> options)
    {
        using (var scope = app.Services.CreateScope())
        {
            var application = new PiranhaApplication(app, scope.ServiceProvider.GetService<IApi>());

            application.Builder
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization();

            options?.Invoke(application);

            // Configure all registered endpoints
            application.Builder.UseEndpoints(endpoints =>
            {
                foreach (var endpoint in application.Endpoints)
                {
                    endpoint.Invoke(endpoints);
                }
            });
        }
        return app;
    }
}