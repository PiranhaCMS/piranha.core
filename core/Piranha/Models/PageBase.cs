/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Piranha.Models
{
    /// <summary>
    /// Base class for page models.
    /// </summary>
    [Serializable]
    public abstract class PageBase : RoutedContentBase, IBlockContent, IMeta, ICommentModel
    {
        /// <summary>
        /// Gets/sets the site id.
        /// </summary>
        public Guid SiteId { get; set; }

        /// <summary>
        /// Gets/sets the optional parent id.
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Gets/sets the sort order of the page in its hierarchical position.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets/sets the navigation title.
        /// </summary>
        [StringLength(128)]
        public string NavigationTitle { get; set; }

        /// <summary>
        /// Gets/sets if the page is hidden in the navigation.
        /// </summary>
        public bool IsHidden { get; set; }

        /// <summary>
        /// Gets/sets the optional redirect.
        /// </summary>
        [StringLength(256)]
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Gets/sets the redirect type.
        /// </summary>
        /// <returns></returns>
        public RedirectType RedirectType { get; set; }

        /// <summary>
        /// Gets/sets the id of the page this page is a copy of
        /// </summary>
        public Guid? OriginalPageId { get; set; }

        /// <summary>
        /// Gets/sets the available blocks.
        /// </summary>
        public IList<Extend.Block> Blocks { get; set; } = new List<Extend.Block>();

        /// <summary>
        /// Gets/sets if comments should be enabled.
        /// </summary>
        /// <value></value>
        public bool EnableComments { get; set; } = false;

        /// <summary>
        /// Gets/sets after how many days after publish date comments
        /// should be closed. A value of 0 means never.
        /// </summary>
        public int CloseCommentsAfterDays { get; set; }

        /// <summary>
        /// Gets/sets the comment count.
        /// </summary>
        public int CommentCount { get; set; }

        /// <summary>
        /// Checks if comments are open for this page.
        /// </summary>
        public bool IsCommentsOpen => EnableComments && Published.HasValue && (CloseCommentsAfterDays == 0 || Published.Value.AddDays(CloseCommentsAfterDays) > DateTime.Now);
    }
}
