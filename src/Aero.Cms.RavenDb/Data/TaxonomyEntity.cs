

namespace Aero.Cms.RavenDb.Data;

[Serializable]
public sealed class Taxonomy : TaxonomyBase
{
    /// <summary>
    /// Gets/sets the id used for grouping.
    /// </summary>
    public string GroupId { get; set; }

    /// <summary>
    /// Gets/sets the taxonomy type.
    /// </summary>
    public TaxonomyType Type { get; set; }
}
