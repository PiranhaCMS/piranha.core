using Piranha.Extend;
using Piranha.Extend.Blocks;

namespace RazorWeb.Models.Blocks
{
    [BlockType(Name = "Html", Category = "Content", Icon = "fab fa-html5", Component = "rawhtml-block")]
    public class RawHtmlBlock : TextBlock
    {
    }
}