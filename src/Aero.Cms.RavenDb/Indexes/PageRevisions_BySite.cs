using Aero.Cms.RavenDb.Data;
using Raven.Client.Documents.Indexes;

namespace Aero.Cms.RavenDb.Indexes;

/// <summary>
/// Static index for detecting page drafts (revisions newer than the page's last save).
/// Uses denormalized SiteId and PageLastModified fields on PageRevision.
/// Covers: PageRepository.GetAllDrafts(), DeleteDraft()
/// </summary>
public class PageRevisions_BySite : AbstractIndexCreationTask<PageRevision, PageRevisions_BySite.Result>
{
    public class Result
    {
        public string SiteId { get; set; }
        public string PageId { get; set; }
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime PageLastModified { get; set; }
        /// <summary>True when this revision was created after the page was last published (i.e. it's a draft).</summary>
        public bool IsDraft { get; set; }
    }

    public PageRevisions_BySite()
    {
        Map = revisions => from r in revisions
                           select new Result
                           {
                               SiteId = r.SiteId,
                               PageId = r.PageId,
                               Id = r.Id,
                               Created = r.Created,
                               PageLastModified = r.PageLastModified,
                               // Precompute the draft condition at index time — RavenDB LINQ does not allow
                               // field-to-field Date comparisons in a Where clause at query time.
                               IsDraft = r.Created > r.PageLastModified
                           };

        Index(x => x.SiteId, FieldIndexing.Exact);
        Index(x => x.PageId, FieldIndexing.Exact);
        Index(x => x.IsDraft, FieldIndexing.Default);
    }
}
