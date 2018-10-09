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
using Microsoft.WindowsAzure.Storage.Auth;
using Piranha;
using Piranha.Azure;

public static class BlobStorageExtensions
{
    /// <summary>
    /// Adds the services for the Azure BlobStorage service.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="credentials">The auth credentials</param>
    /// <param name="containerName">The optional container name</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddPiranhaBlobStorage(this IServiceCollection services,
        StorageCredentials credentials, string containerName = "uploads")
    {
        return services.AddSingleton<IStorage>(new BlobStorage(credentials, containerName));
    }
}
