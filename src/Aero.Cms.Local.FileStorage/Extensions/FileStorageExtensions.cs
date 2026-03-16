

using Microsoft.Extensions.DependencyInjection;
using Aero.Cms;
using Aero.Local;

public static class FileStorageExtensions
{
    /// <summary>
    /// Adds the services for the local FileStorage service.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="basePath">The optional base path for where uploaded media is stored.null Default is wwwroot/uploads/</param>
    /// <param name="baseUrl">The optional base url for accessing uploaded media. Default is ~/uploads/</param>
    /// <param name="naming">How uploaded media files should be named</param>
    /// <param name="generateVersionParam">If a version param should be appended to the public url</param>
    /// <param name="scope">The optional service scope. Default is singleton</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddAeroFileStorage(
        this IServiceCollection services,
        string basePath = null,
        string baseUrl = null,
        FileStorageNaming naming = FileStorageNaming.UniqueFileNames,
        bool generateVersionParam = false,
        ServiceLifetime scope = ServiceLifetime.Singleton)
    {
        App.Modules.Register<FileStorageModule>();

        services.Add(new ServiceDescriptor(typeof(IStorage), sp => new FileStorage(basePath, baseUrl, naming, generateVersionParam), scope));

        return services;
    }
}
