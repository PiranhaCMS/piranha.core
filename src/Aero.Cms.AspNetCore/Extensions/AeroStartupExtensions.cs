

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Aero.Cms;
using Aero.Cms.AspNetCore;
using Aero.Cms.AspNetCore.Hosting;
using Aero.Cms.Security;

/// <summary>
/// Extensions methods for setting up Aero.Cms in Configure
/// and ConfigureServices.
/// </summary>
public static class AeroStartupExtensions
{
    /// <summary>
    /// Adds the services needed to run a Aero.Cms client application.
    /// </summary>
    /// <param name="serviceBuilder">The current service builder</param>
    /// <param name="options">The optional routing options</param>
    /// <returns>The updated service builder</returns>
    public static AeroServiceBuilder UseCms(this AeroServiceBuilder serviceBuilder,
        Action<RoutingOptions> options = null)
    {
        serviceBuilder.Services.AddControllersWithViews();
        var mvcBuilder = serviceBuilder.Services.AddRazorPages();
        if (serviceBuilder.AddRazorRuntimeCompilation)
        {
            mvcBuilder.AddRazorRuntimeCompilation();
        }
        serviceBuilder.Services
            .AddScoped<Aero.Cms.AspNetCore.Services.IApplicationService, Aero.Cms.AspNetCore.Services.ApplicationService>()
            .AddScoped<Aero.Cms.AspNetCore.Services.IModelLoader, Aero.Cms.AspNetCore.Services.ModelLoader>()
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

        serviceBuilder.Services.AddTransient<IStartupFilter, AeroStartupFilter>();
        serviceBuilder.Services.Configure<RoutingOptions>(o => options?.Invoke(o));

        return serviceBuilder;
    }
}
