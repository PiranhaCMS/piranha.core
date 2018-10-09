/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.Services;
using System;

public static class ServiceExtensions
{
    /// <summary>
    /// Adds the default services needed to run Piranha over
    /// Entity Framework Core.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="scope">The optional lifetime</param>
    /// <returns>The updated service collection</returns>
    public static IServiceCollection AddPiranhaEF(this IServiceCollection services,
        ServiceLifetime scope = ServiceLifetime.Scoped)
    {
        services.Add(new ServiceDescriptor(typeof(IContentServiceFactory), typeof(ContentServiceFactory), ServiceLifetime.Singleton));
        services.Add(new ServiceDescriptor(typeof(IDb), typeof(Db), scope));
        services.Add(new ServiceDescriptor(typeof(IApi), typeof(Api), scope));

        return services;
    }

    /// <summary>
    /// Adds the DbContext and the default services needed to run 
    /// Piranha over Entity Framework Core.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="dboptions">The DbContext options builder</param>
    /// <param name="scope">The optional lifetime</param>
    /// <returns>The updated service collection</returns>
    public static IServiceCollection AddPiranhaEF(this IServiceCollection services,
        Action<DbContextOptionsBuilder> dboptions,
        ServiceLifetime scope = ServiceLifetime.Scoped)
    {
        services.AddDbContext<Db>(dboptions);

        return AddPiranhaEF(services, scope);
    }

    /// <summary>
    /// Adds the memory cache service for repository caching.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <returns>The updated service collection</returns>
    public static IServiceCollection AddPiranhaMemCache(this IServiceCollection services)
    {
        return services.AddSingleton<Piranha.ICache, Piranha.Cache.MemCache>();
    }
}
