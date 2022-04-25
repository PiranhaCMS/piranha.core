/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.Cache;

/// <summary>
/// Extension methods for adding MemoryCache to the application.
/// </summary>
public static class PiranhaMemoryCacheExtensions
{
    /// <summary>
    /// Adds the memory cache service to the service collection.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="clone">If returned objects should be cloned</param>
    /// <returns>The updated service collection</returns>
    public static IServiceCollection AddPiranhaMemoryCache(this IServiceCollection services, bool clone = false)
    {
        // Check dependent services
        if (!services.Any(s => s.ServiceType == typeof(IMemoryCache)))
        {
            throw new NotSupportedException("You need to register a IMemoryCache service in order to use Memory Cache in Piranha");
        }

        if (clone)
        {
            return services.AddSingleton<ICache, MemoryCacheWithClone>();
        }
        return services.AddSingleton<ICache, MemoryCache>();
    }

    /// <summary>
    /// Uses the memory cache service in the current application.
    /// </summary>
    /// <param name="serviceBuilder">The current service builder</param>
    /// <param name="clone">If returned objects should be cloned</param>
    /// <returns>The updated service builder</returns>
    public static PiranhaServiceBuilder UseMemoryCache(this PiranhaServiceBuilder serviceBuilder, bool clone = false)
    {
        serviceBuilder.Services.AddPiranhaMemoryCache(clone);

        return serviceBuilder;
    }
}