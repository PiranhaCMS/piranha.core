

namespace Aero.Cms.Manager.Models.Content;

/// <summary>
/// Meta information for fields.
/// </summary>
public class FieldMeta : ContentMeta
{
    /// <summary>
    /// Gets/sets the id.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets/sets if the field should be displayed half width.
    /// </summary>
    public bool IsHalfWidth { get; set; }

    /// <summary>
    /// Gets/sets if the field is translatable or not.
    /// </summary>
    public bool IsTranslatable { get; set; }

    /// <summary>
    /// Gets/sets if this field should notify parent on change.
    /// </summary>
    public bool NotifyChange { get; set; }

    /// <summary>
    /// Gets/sets the field options
    /// </summary>
    public IDictionary<int, string> Options { get; set; } = new Dictionary<int, string>();

    /// <summary>
    /// Gets/sets the optional field settings.
    /// </summary>
    public IDictionary<string, object> Settings { get; set; } = new Dictionary<string, object>();
}
