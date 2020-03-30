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
using Microsoft.Azure.Storage.Auth;
using Piranha;
using Piranha.Azure;

public static class BlobStorageExtensions
{
    /// <summary>
    /// Adds the Azure BlobStorage module.
    /// </summary>
    /// <param name="serviceBuilder">The service builder</param>
    /// <param name="credentials">The auth credentials</param>
    /// <param name="containerName">The optional container name</param>
    /// <param name="scope">The optional service scope. Default is singleton</param>
    /// <returns>The service collection</returns>
    public static PiranhaServiceBuilder UseBlobStorage(this PiranhaServiceBuilder serviceBuilder,
        StorageCredentials credentials, string containerName = "uploads", ServiceLifetime scope = ServiceLifetime.Singleton)
    {
        serviceBuilder.Services.AddPiranhaBlobStorage(credentials, containerName, scope);

        return serviceBuilder;
    }

    /// <summary>
    /// Adds the Azure BlobStorage module.
    /// </summary>
    /// <param name="serviceBuilder">The service builder</param>
    /// <param name="connectionString">The connection string</param>
    /// <param name="containerName">The optional container name</param>
    /// <param name="scope">The optional service scope. Default is singleton</param>
    /// <returns>The service collection</returns>
    public static PiranhaServiceBuilder UseBlobStorage(this PiranhaServiceBuilder serviceBuilder,
        string connectionString, string containerName = "uploads", ServiceLifetime scope = ServiceLifetime.Singleton)
    {
        serviceBuilder.Services.AddPiranhaBlobStorage(connectionString, containerName, scope);

        return serviceBuilder;
    }

    /// <summary>
    /// Adds the services for the Azure BlobStorage service.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="credentials">The auth credentials</param>
    /// <param name="containerName">The optional container name</param>
    /// <param name="scope">The optional service scope. Default is singleton</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddPiranhaBlobStorage(this IServiceCollection services,
        StorageCredentials credentials, string containerName = "uploads", ServiceLifetime scope = ServiceLifetime.Singleton)
    {
        App.Modules.Register<BlobStorageModule>();

        services.Add(new ServiceDescriptor(typeof(IStorage), sp => new BlobStorage(credentials, containerName), scope));

        return services;
    }

    /// <summary>
    /// Adds the services for the Azure BlobStorage service.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="connectionString">The connection string</param>
    /// <param name="containerName">The optional container name</param>
    /// <param name="scope">The optional service scope. Default is singleton</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddPiranhaBlobStorage(this IServiceCollection services,
        string connectionString, string containerName = "uploads", ServiceLifetime scope = ServiceLifetime.Singleton)
    {
        App.Modules.Register<BlobStorageModule>();

        services.Add(new ServiceDescriptor(typeof(IStorage), sp => new BlobStorage(connectionString, containerName), scope));

        return services;
    }
}
