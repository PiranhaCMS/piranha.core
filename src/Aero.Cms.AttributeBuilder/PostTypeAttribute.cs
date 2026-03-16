

namespace Aero.Cms.AttributeBuilder;

/// <summary>
/// Attribute for marking a class as a page type.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class PostTypeAttribute : ContentTypeBaseAttribute
{
    /// <summary>
    /// Gets/sets if the post type should use the block editor
    /// for its main content. The default value is True.
    /// </summary>
    public bool UseBlocks { get; set; }

    /// <summary>
    /// Gets/sets if primary image should be used for the
    /// post type. The default value is true.
    /// </summary>
    public bool UsePrimaryImage { get; set; } = true;

    /// <summary>
    /// Gets/sets if excerpt should be used for the
    /// post type. The default value is true.
    /// </summary>
    public bool UseExcerpt { get; set; } = true;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public PostTypeAttribute() : base()
    {
        UseBlocks = true;
    }
}
