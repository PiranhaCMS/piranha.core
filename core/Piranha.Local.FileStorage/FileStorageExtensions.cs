/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.Local;

public static class FileStorageExtensions
{
    /// <summary>
    /// Adds the services for the local FileStorage service.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="scope">The optional service scope</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddPiranhaFileStorage(this IServiceCollection services,
        ServiceLifetime scope = ServiceLifetime.Singleton, string basePath = null, string baseUrl = null)
    {
        services.Add(new ServiceDescriptor(typeof(IStorage), sp => new FileStorage(basePath, baseUrl), scope));

        return services;
    }
}
