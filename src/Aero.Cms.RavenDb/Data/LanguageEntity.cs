

namespace Aero.Cms.RavenDb.Data;

[Serializable]
public sealed class Language : Entity
{
    /// <summary>
    /// Gets/sets the display title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets/sets the culture.
    /// </summary>
    public string Culture { get; set; }

    /// <summary>
    /// Gets/sets if this is the default language.
    /// </summary>
    public bool IsDefault { get; set; }
}
