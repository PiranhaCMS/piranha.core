

using Aero.Cms.Extend.Fields;

namespace Aero.Cms.Extend.Blocks;

/// <summary>
/// Image block.
/// </summary>
[BlockType(Name = "Image", Category = "Media", Icon = "fas fa-image", Component = "image-block")]
public class ImageBlock : Block
{
    /// <summary>
    /// Gets/sets the image body.
    /// </summary>
    public ImageField Body { get; set; }

    /// <summary>
    /// Gets/sets the selected image aspect.
    /// </summary>
    public SelectField<ImageAspect> Aspect { get; set; } = new SelectField<ImageAspect>();

    /// <inheritdoc />
    public override string GetTitle()
    {
        if (Body != null && Body.Media != null)
        {
            return Body.Media.Filename;
        }
        return "No image selected";
    }
}
