

using Aero.Cms.Extend;

namespace Aero.Cms.Runtime;

public sealed class AppModule : AppDataItem
{
    /// <summary>
    /// Gets/sets the module instance.
    /// </summary>
    public IModule Instance { get; set; }
}
