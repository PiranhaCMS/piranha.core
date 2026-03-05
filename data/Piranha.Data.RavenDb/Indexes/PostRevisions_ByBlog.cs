/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Data.RavenDb.Data;
using Raven.Client.Documents.Indexes;

namespace Piranha.Data.RavenDb.Indexes;

/// <summary>
/// Static index for detecting post drafts (revisions newer than the post's last save).
/// Uses denormalized BlogId and PostLastModified fields on PostRevision.
/// Covers: PostRepository.GetAllDrafts(), DeleteUnusedTags()
/// </summary>
public class PostRevisions_ByBlog : AbstractIndexCreationTask<PostRevision, PostRevisions_ByBlog.Result>
{
    public class Result
    {
        public string BlogId { get; set; }
        public string PostId { get; set; }
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime PostLastModified { get; set; }
        /// <summary>True when this revision was created after the post was last published (i.e. it's a draft).</summary>
        public bool IsDraft { get; set; }
    }

    public PostRevisions_ByBlog()
    {
        Map = revisions => from r in revisions
                           select new Result
                           {
                               BlogId = r.BlogId,
                               PostId = r.PostId,
                               Id = r.Id,
                               Created = r.Created,
                               PostLastModified = r.PostLastModified,
                               // Precompute the draft condition at index time — RavenDB LINQ does not allow
                               // field-to-field Date comparisons in a Where clause at query time.
                               IsDraft = r.Created > r.PostLastModified
                           };

        Index(x => x.BlogId, FieldIndexing.Exact);
        Index(x => x.PostId, FieldIndexing.Exact);
        Index(x => x.IsDraft, FieldIndexing.Default);
    }
}
