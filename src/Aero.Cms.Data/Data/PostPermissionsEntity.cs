

using System.Text.Json.Serialization;

namespace Aero.Cms.Data.Data;

[Serializable]
public sealed class PostPermission
{
    public string PostId { get; set; }
    public string Permission { get; set; }

    [JsonIgnore]
    public Post Post { get; set; }
}
