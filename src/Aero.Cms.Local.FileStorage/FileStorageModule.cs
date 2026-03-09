

using Aero.Cms.Extend;

namespace Aero.Local;

/// <summary>
/// File Storage module.
/// </summary>
public class FileStorageModule : IModule
{
    /// <summary>
    /// Gets the Author
    /// </summary>
    public string Author => "Aero.Cms";

    /// <summary>
    /// Gets the Name
    /// </summary>
    public string Name => "Aero.Cms.Local.FileStorage";

    /// <summary>
    /// Gets the Version
    /// </summary>
    public string Version => Aero.Cms.Utils.GetAssemblyVersion(GetType().Assembly);

    /// <summary>
    /// Gets the description
    /// </summary>
    public string Description => "Module for storing uploaded files on disc with the application.";

    /// <summary>
    /// Gets the package url.
    /// </summary>
    public string PackageUrl => "https://www.nuget.org/packages/Aero.Cms.Local.FileStorage";

    /// <summary>
    /// Gets the icon url.
    /// </summary>
    public string IconUrl => "https://Aerocms.org/assets/twitter-shield.png";

    /// <summary>
    /// Initializes the module.
    /// </summary>
    public void Init() { }
}
