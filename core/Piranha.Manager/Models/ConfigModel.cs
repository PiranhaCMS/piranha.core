/*
 * Copyright (c) .NET Foundation and Contributors
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
        public int ManagerPageSize { get; set; }
        public int ArchivePageSize { get; set; }
        public bool CommentsApprove { get; set; }
        public int CommentsCloseAfterDays { get; set; }
        public bool CommentsEnabledForPosts { get; set; }
        public bool CommentsEnabledForPages { get; set; }
        public int CommentsPageSize { get; set; }
        public int PagesExpires { get; set; }
        public int PostsExpires { get; set; }
        public string MediaCDN { get; set; }
        public int PageRevisions { get; set; }
        public int PostRevisions { get; set; }
        public bool DefaultCollapsedBlocks { get; set; }
        public bool DefaultCollapsedBlockGroupHeaders { get; set; }
    }
}