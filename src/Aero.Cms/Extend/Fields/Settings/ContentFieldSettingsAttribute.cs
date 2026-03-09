

namespace Aero.Cms.Extend.Fields.Settings;

/// <summary>
/// Settings for content fields.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ContentFieldSettingsAttribute : FieldSettingsAttribute
{
    /// <summary>
    /// Gets/sets the currently allowed group.
    /// </summary>
    public string Group { get; set; }
}
