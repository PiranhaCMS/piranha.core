

using Aero.Cms.Extend.Fields;

namespace Aero.Cms.Extend.Blocks;

/// <summary>
/// Video block.
/// </summary>
[BlockType(Name = "Video", Category = "Media", Icon = "fas fa-video", Component = "video-block")]
public class VideoBlock : Block
{
    /// <summary>
    /// Gets/sets the video body.
    /// </summary>
    public VideoField Body { get; set; }

    /// <inheritdoc />
    public override string GetTitle()
    {
        if (Body != null && Body.Media != null)
        {
            return Body.Media.Filename;
        }

        return "No video selected";
    }
}
