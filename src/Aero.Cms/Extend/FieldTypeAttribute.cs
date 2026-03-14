

namespace Aero.Cms.Extend;

/// <summary>
/// Attribute for marking a class as a field type.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class FieldTypeAttribute : Attribute
{
    /// <summary>
    /// Gets/sets the display name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets/sets the optional shorthand for type declaration.
    /// </summary>
    public string Shorthand { get; set; }

    /// <summary>
    /// Gets/sets the name of the component that should be
    /// used to render the field in the manager interface.
    /// </summary>
    public string Component { get; set; }
}
