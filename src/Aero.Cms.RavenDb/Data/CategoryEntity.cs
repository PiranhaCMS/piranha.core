

using System.Text.Json.Serialization;

namespace Aero.Cms.RavenDb.Data;

[Serializable]
public sealed class Category : TaxonomyBase
{
    /// <summary>
    /// Gets/sets the id of the blog page this
    /// category belongs to.
    /// </summary>
    public string BlogId { get; set; }

    /// <summary>
    /// Gets/sets the blog page this category belongs to.
    /// </summary>
    [JsonIgnore]
    public Page Blog { get; set; }
}
