

using Microsoft.Extensions.DependencyInjection;
using Aero.Cms;
using Aero.Local;

public static class FileStorageStartupExtensions
{
    /// <summary>
    /// Adds the FileStorage module if simple startup is used.
    /// </summary>
    /// <param name="serviceBuilder">The service builder</param>
    /// <param name="basePath">The optional base path for where uploaded media is stored.null Default is wwwroot/uploads/</param>
    /// <param name="baseUrl">The optional base url for accessing uploaded media. Default is ~/uploads/</param>
    /// <param name="naming">How uploaded media files should be named</param>
    /// <param name="generateVersionParam">If a version param should be appended to the public url</param>
    /// <param name="scope">The optional service scope. Default is singleton</param>
    /// <returns>The updated builder</returns>
    public static AeroServiceBuilder UseFileStorage(
        this AeroServiceBuilder serviceBuilder,
        string basePath = null,
        string baseUrl = null,
        FileStorageNaming naming = FileStorageNaming.UniqueFileNames,
        bool generateVersionParam = false,
        ServiceLifetime scope = ServiceLifetime.Singleton)
    {
        serviceBuilder.Services.AddAeroFileStorage(basePath, baseUrl, naming, generateVersionParam, scope);

        return serviceBuilder;
    }
}
