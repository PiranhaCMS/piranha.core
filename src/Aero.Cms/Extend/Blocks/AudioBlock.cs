

using Aero.Cms.Extend.Fields;

namespace Aero.Cms.Extend.Blocks;

/// <summary>
/// Audio block.
/// </summary>
[BlockType(Name = "Audio", Category = "Media", Icon = "fas fa-headphones", Component = "audio-block")]
public class AudioBlock : Block
{
    /// <summary>
    /// Gets/sets the Audio body.
    /// </summary>
    public AudioField Body { get; set; }

    /// <inheritdoc />
    public override string GetTitle()
    {
        if (Body != null && Body.Media != null)
        {
            return Body.Media.Filename;
        }

        return "No audio selected";
    }
}
