

namespace Aero.Cms.AttributeBuilder;

/// <summary>
/// Attribute for adding a route to a page type.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class PageTypeArchiveItemAttribute : Attribute
{
    /// <summary>
    /// Gets/sets the allowed post type.
    /// </summary>
    public Type PostType { get; set; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="type">The allowed post type</param>
    public PageTypeArchiveItemAttribute(Type type)
    {
        PostType = type;
    }
}
