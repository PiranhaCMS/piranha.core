

using System.Dynamic;

namespace Aero.Cms.Models;

/// <summary>
/// Dynamic page model.
/// </summary>
[Serializable]
public class DynamicPage : GenericPage<DynamicPage>, IDynamicContent
{
    /// <summary>
    /// Gets/sets the regions.
    /// </summary>
    public dynamic Regions { get; set; } = new ExpandoObject();
}
