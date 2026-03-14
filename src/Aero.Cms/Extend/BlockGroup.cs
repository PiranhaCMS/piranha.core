

namespace Aero.Cms.Extend;

/// <summary>
/// Base class for blocks that can contain other blocks.
/// </summary>
public abstract class BlockGroup : Block
{
    /// <summary>
    /// Gets/sets the available blocks in this group.
    /// </summary>
    public List<Block> Items { get; set; } = new List<Block>();
}
