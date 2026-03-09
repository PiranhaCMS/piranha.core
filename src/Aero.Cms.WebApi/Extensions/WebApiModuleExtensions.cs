

using Microsoft.Extensions.DependencyInjection;
using Aero.Cms;
using Aero.Cms.WebApi;

public static class WebApiModuleExtensions
{
    /// <summary>
    /// Adds the Aero.Cms Api module.
    /// </summary>
    /// <param name="serviceBuilder">The service builder</param>
    /// <param name="configure">The optional api configuration</param>
    /// <returns>The services</returns>
    public static AeroServiceBuilder UseApi(this AeroServiceBuilder serviceBuilder,
        Action<WebApiOptions> configure = null)
    {
        serviceBuilder.Services.AddAeroApi(configure);

        return serviceBuilder;
    }

    /// <summary>
    /// Adds the Aero.Cms Api module.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="configure">The optional api configuration</param>
    /// <returns>The services</returns>
    public static IServiceCollection AddAeroApi(this IServiceCollection services,
        Action<WebApiOptions> configure = null)
    {
        // Configure the api module
        var options = new WebApiOptions();
        configure?.Invoke(options);
        Module.AllowAnonymousAccess = options.AllowAnonymousAccess;

        // Add the api module
        Aero.Cms.App.Modules.Register<Aero.Cms.WebApi.Module>();

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
