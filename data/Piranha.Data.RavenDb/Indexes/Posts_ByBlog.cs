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
