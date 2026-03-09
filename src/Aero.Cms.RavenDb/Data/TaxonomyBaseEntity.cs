

namespace Aero.Cms.RavenDb.Data;

[Serializable]
public abstract class TaxonomyBase : Entity
{
    /// <summary>
    /// Gets/sets the title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets/sets the slug.
    /// </summary>
    public string Slug { get; set; }

    /// <summary>
    /// Gets/sets the created date.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Gets/sets the last modification date.
    /// </summary>
    public DateTime LastModified { get; set; }
}
