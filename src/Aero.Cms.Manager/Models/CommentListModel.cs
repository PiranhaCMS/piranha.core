

namespace Aero.Cms.Manager.Models;

public class CommentListModel
{
    public class CommentItem
    {
        public string Id { get; set; }
        public string Author { get; set; }
        public string AuthorImage { get; set; }
        public string Email { get; set; }
        public string Body { get; set; }
        public string ArticleTitle { get; set; }
        public string ArticleUrl { get; set; }
        public bool IsApproved { get; set; }
        public string Created { get; set; }
        internal DateTime CreatedDate { get; set; }
    }

    /// <summary>
    /// Gets/sets the optionally select content id.
    /// </summary>
    public string? ContentId { get; set; }

    /// <summary>
    /// Gets/sets the available comments.
    /// </summary>
    public List<CommentItem> Comments { get; set; } = new List<CommentItem>();

    /// <summary>
    /// Gets/sets the optional status message from the last operation.
    /// </summary>
    public StatusMessage Status { get; set; }
}
