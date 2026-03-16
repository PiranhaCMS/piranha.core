

using Aero.Cms.Extend.Fields;

namespace Aero.Cms.Models;

/// <summary>
/// Base class for generic content.
/// </summary>
public abstract class GenericContent : ContentBase
{
    /// <summary>
    /// Gets/sets the optional primary image.
    /// </summary>
    public ImageField PrimaryImage { get; set; } = new ImageField();

    /// <summary>
    /// Gets/sets the optional excerpt.
    /// </summary>
    public string Excerpt { get; set; }
}
