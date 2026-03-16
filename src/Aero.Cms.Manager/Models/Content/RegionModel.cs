

namespace Aero.Cms.Manager.Models.Content;

/// <summary>
/// Edit model for a region.
/// </summary>
public class RegionModel
{
    /// <summary>
    /// Gets/sets the available items. A region collection can have several items,
    /// a regular region will only have one item in the collection.
    /// </summary>
    public List<RegionItemModel> Items { get; set; } = new List<RegionItemModel>();

    /// <summary>
    /// Gets/sets the meta information.
    /// </summary>
    public RegionMeta Meta { get; set; }
}
