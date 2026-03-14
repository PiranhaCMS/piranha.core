

namespace Aero.Cms.Data.Data;

[Serializable]
public sealed class PostComment : Comment
{
    /// <summary>
    /// Gets/sets the post id.
    /// </summary>
    public string PostId { get; set; }

    /// <summary>
    /// Gets/sets the post.
    /// </summary>
    public Post Post { get; set; }
}
