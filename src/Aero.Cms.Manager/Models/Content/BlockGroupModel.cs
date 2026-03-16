

namespace Aero.Cms.Manager.Models.Content;

/// <summary>
/// Edit model for block groups.
/// </summary>
public class BlockGroupModel : BlockModel
{
    /// <summary>
    /// Gets/sets the type of the block group.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Gets/sets the available child items in the group.
    /// </summary>
    public List<BlockModel> Items { get; set; } = new List<BlockModel>();

    /// <summary>
    /// Gets/sets the available global group fields.
    /// </summary>
    public List<FieldModel> Fields { get; set; } = new List<FieldModel>();
}
