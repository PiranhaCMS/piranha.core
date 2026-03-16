

using Aero.Cms.Extend.Fields;

namespace Aero.Cms.Extend.Blocks;

/// <summary>
/// Block for referencing a page.
/// </summary>
[BlockType(Name = "Page link", Category = "References", Icon = "fas fa-link", Component = "page-block")]
public class PageBlock : Block
{
    /// <summary>
    /// Gets/sets the page link.
    /// </summary>
    public PageField Body { get; set; }

    /// <inheritdoc />
    public override string GetTitle()
    {
        if (Body != null && Body.Page != null)
        {
            return Body.Page.Title;
        }
        return "No page selected";
    }
}
