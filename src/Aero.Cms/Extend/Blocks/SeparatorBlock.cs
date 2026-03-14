

namespace Aero.Cms.Extend.Blocks;

/// <summary>
/// Separator block.
/// </summary>
[BlockType(Name = "Separator", Category = "Content", Icon = "fas fa-divide", Component = "separator-block")]
public class SeparatorBlock : Block
{
    /// <inheritdoc />
    public override string GetTitle()
    {
        return "----";
    }
}
