

namespace Aero.Cms.Data.Data;

public class PostRevision : ContentRevisionBase
{
    /// <summary>
    /// Gets/sets the id of the post this revision belongs to.
    /// </summary>
    public string PostId { get; set; }

    /// <summary>
    /// Gets/sets the blog id, denormalized from the parent Post
    /// to avoid cross-collection navigation queries.
    /// </summary>
    public string BlogId { get; set; }

    /// <summary>
    /// Gets/sets the snapshot of the post's LastModified at the
    /// time this revision was created. Used for draft detection
    /// without loading the parent Post document.
    /// </summary>
    public DateTime PostLastModified { get; set; }
}
