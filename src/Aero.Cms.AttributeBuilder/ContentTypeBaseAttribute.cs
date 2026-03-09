

namespace Aero.Cms.AttributeBuilder;

/// <summary>
/// Abstract class for building content types.
/// </summary>
public abstract class ContentTypeBaseAttribute : Attribute
{
    /// <summary>
    /// Gets/sets the unique id.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets/sets the optional title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets/sets the optional description.
    /// </summary>
    public string Description { get; set; }
}
