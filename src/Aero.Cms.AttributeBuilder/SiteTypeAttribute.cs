

namespace Aero.Cms.AttributeBuilder;

/// <summary>
/// Attribute for marking a class as a page type.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class SiteTypeAttribute : ContentTypeBaseAttribute
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public SiteTypeAttribute() : base() { }
}
