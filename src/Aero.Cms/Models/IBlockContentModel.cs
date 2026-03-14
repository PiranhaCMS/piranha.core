

namespace Aero.Cms.Models;

public interface IBlockContent
{
    /// <summary>
    /// Gets/sets the blocks.
    /// </summary>
    List<Extend.Block> Blocks { get; set; }
}
