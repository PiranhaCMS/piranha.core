

using System.Dynamic;

namespace Aero.Cms.Models;

/// <summary>
/// Dynamic page model.
/// </summary>
[Serializable]
public class DynamicPost : Post<DynamicPost>, IDynamicContent
{
    /// <summary>
    /// Gets/sets the regions.
    /// </summary>
    public dynamic Regions { get; set; } = new ExpandoObject();
}
