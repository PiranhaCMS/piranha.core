using Aero.Cms.Data.Data;
using Raven.Client.Documents.Indexes;

namespace Aero.Cms.Data.Indexes;

/// <summary>
/// Static index for filtering posts by tag slug within a blog.
/// Uses a fan-out map over the Tags array embedded in each Post document.
/// Covers: ArchiveRepository tag-based filtering.
/// </summary>
public class Posts_ByTag : AbstractIndexCreationTask<Post, Posts_ByTag.Result>
{
    public class Result
    {
        public string BlogId { get; set; }
        public string TagSlug { get; set; }
        public DateTime? Published { get; set; }
    }

    public Posts_ByTag()
    {
        Map = posts => from p in posts
                       from tag in p.Tags
                       select new Result
                       {
                           BlogId = p.BlogId,
                           // PostTag embeds the Tag object; access Slug through Tag.Slug
                           TagSlug = tag.Tag.Slug,
                           Published = p.Published
                       };

        Index(x => x.BlogId, FieldIndexing.Exact);
        Index(x => x.TagSlug, FieldIndexing.Exact);
    }
}
