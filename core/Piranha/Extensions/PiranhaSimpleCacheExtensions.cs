/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.Cache;

/// <summary>
/// Extension methods for adding SimpleCache to the application.
/// </summary>
public static class PiranhaSimpleCacheExtensions
{
    /// <summary>
    /// Adds the simple cache service to the service collection.
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