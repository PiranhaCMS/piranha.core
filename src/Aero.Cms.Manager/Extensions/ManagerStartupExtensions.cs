using Aero.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Aero.Cms;
using Aero.Cms.Manager;
using Microsoft.AspNetCore.Builder;

public static class ManagerStartupExtensions
{
    /// <summary>
    /// Uses the Aero.Cms Manager services if simple startup is used.
    /// </summary>
    /// <param name="serviceBuilder">The service builder</param>
    /// <param name="options">The optional options</param>
    /// <param name="jsonOptions">Optional JSON options</param>
    /// <returns>The updated builder</returns>
    public static AeroServiceBuilder UseManager(this AeroServiceBuilder serviceBuilder,
        Action<ManagerOptions> options = null,
        Action<JsonOptions> jsonOptions = null)
    {
        // Perform optional configuration
        var managerOptions = new ManagerOptions();
        options?.Invoke(managerOptions);

        // Add manager services
        serviceBuilder.Services.AddAeroManager();

        // Add dependent ASP.NET services
        serviceBuilder.Services.AddLocalization(o =>
            o.ResourcesPath = "Resources"
        );
        serviceBuilder.Services.AddControllersWithViews();
        serviceBuilder.Services.AddRazorPages()
            .AddAeroManagerOptions(jsonOptions);
        serviceBuilder.Services.AddAntiforgery(o =>
        {
            o.HeaderName = managerOptions.XsrfHeaderName;
        });

        // Add options
        serviceBuilder.Services.Configure<ManagerOptions>(o =>
        {
            o.JsonOptions = managerOptions.JsonOptions;
            o.XsrfCookieName = managerOptions.XsrfCookieName;
            o.XsrfHeaderName = managerOptions.XsrfHeaderName;
        });
        return serviceBuilder;
    }

    /// <summary>
    /// Uses the Aero.Cms Manager if simple startup is enabled.
    /// </summary>
    /// <param name="applicationBuilder">The application builder</param>
    /// <returns>The updated builder</returns>
    public static AeroApplicationBuilder UseManager(this AeroApplicationBuilder applicationBuilder)
    {
        applicationBuilder.Builder.UseAeroManager();

        applicationBuilder.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            endpoints.MapAeroManager();
        });
        return applicationBuilder;
    }
}
