

namespace Aero.Cms.Manager.Models.Content;

/// <summary>
/// Meta information for regions.
/// </summary>
public class RegionMeta : ContentMeta
{
    /// <summary>
    /// Gets/sets the id.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets/sets if  this is a collection region.
    /// </summary>
    public bool IsCollection { get; set; }

    /// <summary>
    /// Gets/sets if the items in the collection should be expanded
    /// </summary>
    public bool Expanded { get; set; }

    /// <summary>
    /// Gets/sets how the region should be display (content/hidden/setting).
    /// </summary>
    public string Display { get; set; }

    /// <summary>
    /// Gets/sets the editor width.
    /// </summary>
    public string Width { get; set; }
}
