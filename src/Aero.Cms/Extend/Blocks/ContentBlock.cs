

using Aero.Cms.Extend.Fields;

namespace Aero.Cms.Extend.Blocks;

/// <summary>
/// Block for referencing a content model.
/// </summary>
[BlockType(Name = "Content link", Category = "References", Icon = "fas fa-link", Component = "content-block")]
public class ContentBlock : Block
{
    /// <summary>
    /// Gets/sets the page link.
    /// </summary>
    public ContentField Body { get; set; }

    /// <inheritdoc />
    public override string GetTitle()
    {
        if (Body != null && Body.Content != null)
        {
            return Body.Content.Title;
        }
        return "No content selected";
    }
}
