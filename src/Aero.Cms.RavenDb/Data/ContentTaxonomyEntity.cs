

using System.Text.Json.Serialization;

namespace Aero.Cms.RavenDb.Data;

[Serializable]
public sealed class ContentTaxonomy
{
    /// <summary>
    /// Gets/sets the content id.
    /// </summary>
    public string ContentId { get; set; }

    /// <summary>
    /// Gets/sets the taxonomy id.
    /// </summary>
    public string TaxonomyId { get; set; }

    /// <summary>
    /// Gets/sets the content.
    /// </summary>
    [JsonIgnore]
    public Content Content { get; set; }

    /// <summary>
    /// Gets/sets the taxonomy.
    /// </summary>
    public Taxonomy Taxonomy { get; set; }
}
