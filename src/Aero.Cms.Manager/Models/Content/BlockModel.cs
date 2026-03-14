

namespace Aero.Cms.Manager.Models.Content;

/// <summary>
/// Edit model for blocks.
/// </summary>
public abstract class BlockModel : Entity
{
    /// <summary>
    /// Gets/sets the meta information.
    /// </summary>
    public BlockMeta Meta { get; set; }
}
