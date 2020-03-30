/*
 * Copyright (c) .NET Foundation and Contributors
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
    /// <param name="basePath">The optional base path for where uploaded media is stored.null Default is wwwroot/uploads/</param>
    /// <param name="baseUrl">The optional base url for accessing uploaded media. Default is ~/uploads/</param>
    /// <param name="scope">The optional service scope. Default is singleton</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddPiranhaFileStorage(this IServiceCollection services,
        string basePath = null, string baseUrl = null, ServiceLifetime scope = ServiceLifetime.Singleton)
    {
        App.Modules.Register<FileStorageModule>();

        services.Add(new ServiceDescriptor(typeof(IStorage), sp => new FileStorage(basePath, baseUrl), scope));

        return services;
    }
}
