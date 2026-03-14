

namespace Aero.Cms.Models;

[Serializable]
public sealed class ContentTypeField
{
    /// <summary>
    /// Gets/sets the id.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets/sets the optional title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets/sets the value type.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Gets/sets the options.
    /// </summary>
    public FieldOption Options { get; set; }

    /// <summary>
    /// Gets/sets the optional placeholder for
    /// text based fields.
    /// </summary>
    public string Placeholder { get; set; }

    /// <summary>
    /// Gets/sets the optional description to be shown in
    /// the manager interface.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets/sets the available field settings.
    /// </summary>
    public IDictionary<string, object> Settings = new Dictionary<string, object>();
}
