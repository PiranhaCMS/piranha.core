

namespace Aero.Cms.Manager.Models;

/// <summary>
/// Modules model.
/// </summary>
public class ModuleListModel
{
    /// <summary>
    /// A list item in the module model.
    /// </summary>
    public class ModuleItem
    {
        public string Author { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string PackageUrl { get; set; }
        public string IconUrl { get; set; }
    }

    /// <summary>
    /// Gets/set the available items.
    /// </summary>
    public List<ModuleItem> Items { get; set; } = new List<ModuleItem>();
}
