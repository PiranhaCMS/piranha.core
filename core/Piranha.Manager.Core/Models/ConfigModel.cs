/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Manager.Models
{
    /// <summary>
    /// Config model.
    /// </summary>
    public class ConfigModel
    {
        public bool HierarchicalPageSlugs { get; set; }
        public int ExpandedSitemapLevels { get; set; }
        public int ArchivePageSize { get; set; }
        public int PagesExpires { get; set; }
        public int PostsExpires { get; set; }
        public string MediaCDN { get; set; }
        public int PageRevisions { get; set; }
        public int PostRevisions { get; set; }
    }
}