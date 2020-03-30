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
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.Cache;
using Piranha.Services;

public static class PiranhaExtensions
{
    public static IServiceCollection AddPiranha(this IServiceCollection services,
        ServiceLifetime scope = ServiceLifetime.Scoped)
    {
        services.Add(new ServiceDescriptor(typeof(IContentFactory), typeof(ContentFactory), ServiceLifetime.Singleton));
        services.Add(new ServiceDescriptor(typeof(IApi), typeof(Api), scope));

        return services;
    }

    /// <summary>
    /// Adds the distributed cache service for repository caching.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <returns>The updated service collection</returns>
    public static IServiceCollection AddPiranhaDistributedCache(this IServiceCollection services)
    {
        return services.AddSingleton<ICache, DistributedCache>();
    }

    public static PiranhaServiceBuilder UseMemoryCache(this PiranhaServiceBuilder serviceBuilder, bool clone = false)
    {
        // Check dependent services
        if (!serviceBuilder.Services.Any(s => s.ServiceType == typeof(IMemoryCache)))
        {
            throw new NotSupportedException("You need to register a IMemoryCache service in order to use Memory Cache in Piranha");
        }
        serviceBuilder.Services.AddPiranhaMemoryCache();

        return serviceBuilder;
    }

    /// <summary>
    /// Adds the memory cache service for repository caching.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="clone">If returned objects should be cloned</param>
    /// <returns>The updated service collection</returns>
    public static IServiceCollection AddPiranhaMemoryCache(this IServiceCollection services, bool clone = false)
    {
        if (clone)
        {
            return services.AddSingleton<ICache, MemoryCacheWithClone>();
        }
        return services.AddSingleton<ICache, MemoryCache>();
    }

    /// <summary>
    /// Adds the simple cache service for repository caching.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="clone">If returned objects should be cloned</param>
    /// <returns>The updated service collection</returns>
    public static IServiceCollection AddPiranhaSimpleCache(this IServiceCollection services, bool clone = false)
    {
        if (clone)
        {
            return services.AddSingleton<ICache, SimpleCacheWithClone>();
        }
        return services.AddSingleton<ICache, SimpleCache>();
    }
}
