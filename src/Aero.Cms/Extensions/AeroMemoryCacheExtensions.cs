

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Aero.Cms;
using Aero.Cms.Cache;

/// <summary>
/// Extension methods for adding MemoryCache to the application.
/// </summary>
public static class AeroMemoryCacheExtensions
{
    /// <summary>
    /// Adds the memory cache service to the service collection.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <returns>The updated service collection</returns>
    public static IServiceCollection AddAeroMemoryCache(this IServiceCollection services)
    {
        // Check dependent services
        if (!services.Any(s => s.ServiceType == typeof(IMemoryCache)))
        {
            throw new NotSupportedException("You need to register a IMemoryCache service in order to use Memory Cache in Aero.Cms");
        }
        return services.AddSingleton<ICache, Aero.Cms.Cache.MemoryCache>();
    }

    /// <summary>
    /// Uses the memory cache service in the current application.
    /// </summary>
    /// <param name="serviceBuilder">The current service builder</param>
    /// <returns>The updated service builder</returns>
    public static AeroServiceBuilder UseMemoryCache(this AeroServiceBuilder serviceBuilder)
    {
        serviceBuilder.Services.AddAeroMemoryCache();

        return serviceBuilder;
    }
}