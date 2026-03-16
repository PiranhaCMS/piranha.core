

using System.Text.Json.Serialization;

namespace Aero.Cms.Data.Data;

[Serializable]
public sealed class PostTag
{
    /// <summary>
    /// Gets/sets the post id.
    /// </summary>
    public string PostId { get; set; }

    /// <summary>
    /// Gets/sets the tag id.
    /// </summary>
    public string TagId { get; set; }

    /// <summary>
    /// Gets/sets the post.
    /// </summary>
    [JsonIgnore]
    public Post Post { get; set; }

    /// <summary>
    /// Gets/sets the tag.
    /// </summary>
    public Tag Tag { get; set; }
}
