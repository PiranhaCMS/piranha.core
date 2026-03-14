

using Aero.Cms.Extend;

namespace Aero.Cms.Manager.Models.Content;

/// <summary>
/// Edit model for block groups.
/// </summary>
public class BlockItemModel : BlockModel
{
    /// <summary>
    /// Gets/sets if the block should be active
    /// part of a group.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets/sets the block model.
    /// </summary>
    /// <value></value>
    public Block Model { get; set; }
}
