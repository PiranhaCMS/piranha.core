using Aero.Cms.Data.Data;
using Raven.Client.Documents.Indexes;

namespace Aero.Cms.Data.Indexes;

/// <summary>
/// Index for detecting post revisions that are newer than the published post.
/// Uses denormalized BlogId and PostLastModified fields on PostRevision.
/// NOTE: This index is still used by DeleteUnusedCategories in PostRepository.
/// </summary>
public class Revisions_ByIsNewerThanPost
    : AbstractIndexCreationTask<PostRevision, Revisions_ByIsNewerThanPost.Result>
{
    public class Result
    {
        public string BlogId { get; set; }
        public bool IsNewer { get; set; }
        public string Id { get; set; }
    }

    public Revisions_ByIsNewerThanPost()
    {
        Map = revisions => from r in revisions
                           select new
                           {
                               // Use denormalized fields \u2014 PostRevision.Post navigation was removed
                               BlogId = r.BlogId,
                               IsNewer = r.Created > r.PostLastModified,
                               Id = r.Id
                           };
    }
}
