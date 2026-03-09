

using System.Text.Json.Serialization;

namespace Aero.Cms.RavenDb.Data;

[Serializable]
public sealed class PostField : ContentFieldBase
{
    /// <summary>
    /// Gets/sets the post id.
    /// </summary>
    public string PostId { get; set; }

    /// <summary>
    /// Gets/sets the post.
    /// </summary>
    [JsonIgnore]
    public Post Post { get; set; }
}
