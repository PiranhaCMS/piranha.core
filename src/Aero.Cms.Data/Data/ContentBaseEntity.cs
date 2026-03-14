

namespace Aero.Cms.Data.Data;

[Serializable]
public abstract class ContentBase<T> : Entity where T : ContentFieldBase
{
    /// <summary>
    /// Gets/sets the main title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets/sets the created date.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Gets/sets the last modification date.
    /// </summary>
    public DateTime LastModified { get; set; }

    /// <summary>
    /// Gets/sets the available fields.
    /// </summary>
    public List<T> Fields { get; set; } = new List<T>();
}
