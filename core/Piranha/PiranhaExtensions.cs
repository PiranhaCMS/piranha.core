/*
 * Copyright (c) 2018-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
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
    /// Adds the memory cache service for repository caching.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <returns>The updated service collection</returns>
    [Obsolete("Please use AddPiranhaSimpleCache instead", true)]
    public static IServiceCollection AddPiranhaMemCache(this IServiceCollection services)
    {
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

    /// <summary>
    /// Adds the memory cache service for repository caching.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <returns>The updated service collection</returns>
    public static IServiceCollection AddPiranhaMemoryCache(this IServiceCollection services)
    {
        return services.AddSingleton<ICache, MemoryCache>();
    }

    /// <summary>
    /// Adds the simple cache service for repository caching.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <returns>The updated service collection</returns>
    public static IServiceCollection AddPiranhaSimpleCache(this IServiceCollection services)
    {
        return services.AddSingleton<ICache, SimpleCache>();
    }
}
