

using Aero.Cms.RavenDb.Data;
using Raven.Client.Documents.Indexes;

namespace Aero.Cms.RavenDb.Indexes;

/// <summary>
/// Static index for querying posts within a blog.
/// Covers: archive listing, post by slug, category filtering.
/// </summary>
public class Posts_ByBlog : AbstractIndexCreationTask<Post>
{
    public class Result
    {
        public string BlogId { get; set; }
        public string Slug { get; set; }
        public string CategoryId { get; set; }
        public DateTime? Published { get; set; }
    }

    public Posts_ByBlog()
    {
        Map = posts => from p in posts
                       select new Result
                       {
                           BlogId = p.BlogId,
                           Slug = p.Slug,
                           CategoryId = p.CategoryId,
                           Published = p.Published
                       };

        Index(x => x.BlogId, FieldIndexing.Exact);
        Index(x => x.Slug, FieldIndexing.Exact);
    }
}
