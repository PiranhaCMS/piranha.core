

using System.Text.Json.Serialization;

namespace Aero.Cms.RavenDb.Data;

[Serializable]
public sealed class PagePermission
{
    public string PageId { get; set; }
    public string Permission { get; set; }

    [JsonIgnore]
    public Page Page { get; set; }
}
