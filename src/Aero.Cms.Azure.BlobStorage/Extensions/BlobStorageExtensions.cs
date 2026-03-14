

using Aero.Azure;
using Azure.Core;
using Microsoft.Extensions.DependencyInjection;
using Aero.Cms;

public static class BlobStorageExtensions
{
    /// <summary>
    /// Adds the Azure BlobStorage module.
    /// </summary>
    /// <param name="serviceBuilder">The service builder</param>
    /// <param name="credential">The token credential used to sign requests.</param>
    /// <param name="blobContainerUri">
    /// A <see cref="Uri"/> referencing the blob service.
    /// This is likely to be similar to "https://{account_name}.blob.core.windows.net".
    /// </param>
    /// <param name="naming">How uploaded media files should be named</param>
    /// <param name="scope">The optional service scope. Default is singleton</param>
    /// <returns>The service collection</returns>
    public static AeroServiceBuilder UseBlobStorage(
        this AeroServiceBuilder serviceBuilder,
        Uri blobContainerUri,
        TokenCredential credential,
        BlobStorageNaming naming = BlobStorageNaming.UniqueFileNames,
        ServiceLifetime scope = ServiceLifetime.Singleton)
    {
        serviceBuilder.Services.AddAeroBlobStorage(blobContainerUri, credential, naming, scope);

        return serviceBuilder;
    }

    /// <summary>
    /// Adds the Azure BlobStorage module.
    /// </summary>
    /// <param name="serviceBuilder">The service builder</param>
    /// <param name="connectionString">The connection string</param>
    /// <param name="containerName">The optional container name</param>
    /// <param name="naming">How uploaded media files should be named</param>
    /// <param name="scope">The optional service scope. Default is singleton</param>
    /// <returns>The service collection</returns>
    public static AeroServiceBuilder UseBlobStorage(
        this AeroServiceBuilder serviceBuilder,
        string connectionString,
        string containerName = "uploads",
        BlobStorageNaming naming = BlobStorageNaming.UniqueFileNames,
        ServiceLifetime scope = ServiceLifetime.Singleton)
    {
        serviceBuilder.Services.AddAeroBlobStorage(connectionString, containerName, naming, scope);

        return serviceBuilder;
    }

    /// <summary>
    /// Adds the services for the Azure BlobStorage service.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="credential">The token credential used to sign requests.</param>
    /// <param name="blobContainerUri">
    /// A <see cref="Uri"/> referencing the blob service.
    /// This is likely to be similar to "https://{account_name}.blob.core.windows.net".
    /// </param>
    /// <param name="naming">How uploaded media files should be named</param>
    /// <param name="scope">The optional service scope. Default is singleton</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddAeroBlobStorage(
        this IServiceCollection services,
        Uri blobContainerUri,
        TokenCredential credential,
        BlobStorageNaming naming = BlobStorageNaming.UniqueFileNames,
        ServiceLifetime scope = ServiceLifetime.Singleton)
    {
        App.Modules.Register<BlobStorageModule>();

        services.Add(new ServiceDescriptor(typeof(IStorage), sp =>
            new BlobStorage(blobContainerUri, credential, naming), scope));

        return services;
    }

    /// <summary>
    /// Adds the services for the Azure BlobStorage service.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="connectionString">The connection string</param>
    /// <param name="containerName">The optional container name</param>
    /// <param name="naming">How uploaded media files should be named</param>
    /// <param name="scope">The optional service scope. Default is singleton</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddAeroBlobStorage(
        this IServiceCollection services,
        string connectionString,
        string containerName = "uploads",
        BlobStorageNaming naming = BlobStorageNaming.UniqueFileNames,
        ServiceLifetime scope = ServiceLifetime.Singleton)
    {
        App.Modules.Register<BlobStorageModule>();

        services.Add(new ServiceDescriptor(typeof(IStorage), sp =>
            new BlobStorage(connectionString, containerName, naming), scope));

        return services;
    }
}
