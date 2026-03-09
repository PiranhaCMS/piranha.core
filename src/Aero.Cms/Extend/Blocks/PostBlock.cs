

using Aero.Cms.Extend.Fields;

namespace Aero.Cms.Extend.Blocks;

/// <summary>
/// Block for referencing a post.
/// </summary>
[BlockType(Name = "Post link", Category = "References", Icon = "fas fa-link", Component = "post-block")]
public class PostBlock : Block
{
    /// <summary>
    /// Gets/sets the post link.
    /// </summary>
    public PostField Body { get; set; }

    /// <inheritdoc />
    public override string GetTitle()
    {
        if (Body != null && Body.Post != null)
        {
            return Body.Post.Title;
        }
        return "No post selected";
    }
}
