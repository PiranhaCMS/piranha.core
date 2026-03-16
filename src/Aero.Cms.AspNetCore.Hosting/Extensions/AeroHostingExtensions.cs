

using Aero.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Aero.Cms;

/// <summary>
/// Extensions methods for hosting Aero.Cms in an ASP.NET
/// application.
/// </summary>
public static class AeroHostingExtensions
{
    /// <summary>
    /// Adds Aero.Cms to the web application builder.
    /// </summary>
    /// <param name="builder">The web application builder</param>
    /// <param name="options">The startup options</param>
    /// <returns>The WebApplicationBuilder</returns>
    public static WebApplicationBuilder AddAero(this WebApplicationBuilder builder, Action<AeroServiceBuilder> options)
    {
        builder.Services.AddAero(options);
        return builder;
    }

    /// <summary>
    /// Simple startup with integrated middleware that also adds common
    /// dependencies needed for an integrated web application.
    /// </summary>
    /// <param name="builder">The application builder</param>
    /// <param name="options">Action for configuring the builder</param>
    /// <returns>The updated application builder</returns>
    public static IApplicationBuilder UseAero(this IApplicationBuilder builder, Action<AeroApplicationBuilder> options)
    {
        var applicationBuilder = new AeroApplicationBuilder(builder);

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
    /// Uses the Aero.Cms runtime components for ASP.NET and sets up
    /// common dependencies needed for an integrated web application.
    /// </summary>
    /// <param name="app">The web application</param>
    /// <param name="options">Action for configuring the application</param>
    /// <returns>The web application</returns>
    public static WebApplication UseAero(this WebApplication app, Action<AeroApplication> options)
    {
        using (var scope = app.Services.CreateScope())
        {
            var application = new AeroApplication(app, scope.ServiceProvider.GetService<IApi>());

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