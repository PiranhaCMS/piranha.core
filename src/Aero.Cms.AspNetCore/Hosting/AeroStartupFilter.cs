

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Aero.Cms.AspNetCore.Http;

namespace Aero.Cms.AspNetCore.Hosting;

/// <summary>
/// Startup filter for adding application routing to the beginning
/// of the pipeline.
/// </summary>
internal class AeroStartupFilter : IStartupFilter
{
    /// <summary>
    /// Configures the application builder.
    /// </summary>
    /// <param name="next">The next filter</param>
    /// <returns>The configure action</returns>
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return builder =>
        {
            builder
                .UseSecurityMiddleware()
                .UseStaticFiles()
                .UseMiddleware<RoutingMiddleware>()
                .UseMiddleware<SitemapMiddleware>();
            next(builder);
        };
    }
}
