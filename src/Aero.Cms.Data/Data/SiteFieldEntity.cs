

namespace Aero.Cms.Data.Data;

[Serializable]
public sealed class SiteField : ContentFieldBase
{
    /// <summary>
    /// Gets/sets the site id.
    /// </summary>
    public string SiteId { get; set; }

    /// <summary>
    /// Gets/sets the site.
    /// </summary>
    public Site Site { get; set; }
}
