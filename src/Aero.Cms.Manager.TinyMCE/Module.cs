

using System.Reflection;
using Aero.Cms.Extend;


namespace Aero.Cms.Manager.TinyMCE;

/// <summary>
/// Module definition for the TinyMCE module.
/// </summary>
public sealed class Module : IModule
{
    /// <summary>
    /// Gets the Author
    /// </summary>
    public string Author => "Aero.Cms";

    /// <summary>
    /// Gets the Name
    /// </summary>
    public string Name => "Aero.Cms.Manager.TinyMCE";

    /// <summary>
    /// Gets the Version
    /// </summary>
    public string Version => Aero.Cms.Utils.GetAssemblyVersion(this.GetType().Assembly);

    /// <summary>
    /// Gets the description
    /// </summary>
    public string Description => "Tiny MCE WYSIWYG Editor for Aero Cms.";

    /// <summary>
    /// Gets the package url.
    /// </summary>
    public string PackageUrl => "https://www.nuget.org/packages/Aero.Cms.Manager.TinyMCE";

    /// <summary>
    /// Gets the icon url.
    /// </summary>
    public string IconUrl => "https://Aerocms.org/assets/twitter-shield.png";

    /// <summary>
    /// The assembly.
    /// </summary>
    internal static readonly Assembly Assembly;

    /// <summary>
    /// Static initialization.
    /// </summary>
    static Module()
    {
        // Get assembly information
        Assembly = typeof(Module).GetTypeInfo().Assembly;
    }

    /// <summary>
    /// Initializes the module.
    /// </summary>
    public void Init() { }
}
