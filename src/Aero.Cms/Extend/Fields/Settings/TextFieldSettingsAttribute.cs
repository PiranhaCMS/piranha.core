

namespace Aero.Cms.Extend.Fields.Settings;

/// <summary>
/// Settings for text fields.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class TextFieldSettingsAttribute : FieldSettingsAttribute
{
    /// <summary>
    /// Gets/sets the optional default value for field.
    /// </summary>
    public string DefaultValue { get; set; }

    /// <summary>
    /// Gets/sets the optional max length of the field. A value
    /// of 0 means that the field has no max length.
    /// </summary>
    public int MaxLength { get; set; }
}
