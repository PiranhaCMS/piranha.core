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
/// Static index for querying pages within a site.
/// Covers: sitemap listing, navigation, slug lookups, archive filtering.
/// </summary>
public class Pages_BySite : AbstractIndexCreationTask<Page>
{
    public class Result
    {
        public string SiteId { get; set; }
        public string ParentId { get; set; }
        public int SortOrder { get; set; }
        public string Slug { get; set; }
        public string PageTypeId { get; set; }
        public string ContentType { get; set; }
        public DateTime? Published { get; set; }
    }

    public Pages_BySite()
    {
        Map = pages => from p in pages
                       select new Result
                       {
                           SiteId = p.SiteId,
                           ParentId = p.ParentId,
                           SortOrder = p.SortOrder,
                           Slug = p.Slug,
                           PageTypeId = p.PageTypeId,
                           ContentType = p.ContentType,
                           Published = p.Published
                       };

        Index(x => x.SiteId, FieldIndexing.Exact);
        Index(x => x.Slug, FieldIndexing.Exact);
    }
}
