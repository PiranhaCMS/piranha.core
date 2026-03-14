

namespace Aero.Cms.AttributeBuilder;

/// <summary>
/// Attribute for marking a class as a content type.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ContentTypeAttribute : ContentTypeBaseAttribute
{
    /// <summary>
    /// Gets/sets if excerpt should be used for the
    /// content type. The default value is true.
    /// </summary>
    public bool UseExcerpt { get; set; } = true;

    /// <summary>
    /// Gets/sets if primary image should be used for the
    /// content type. The default value is true.
    /// </summary>
    public bool UsePrimaryImage { get; set; } = true;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public ContentTypeAttribute() : base() { }
}
