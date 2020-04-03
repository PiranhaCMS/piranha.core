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
using Piranha.Extend.Fields;

namespace Piranha.Models
{
    /// <summary>
    /// Base class for post models.
    /// </summary>
    [Serializable]
    public abstract class PostBase : RoutedContentBase, IBlockContent, IMeta, ICommentModel
    {
        /// <summary>
        /// Gets/sets the blog page id.
        /// </summary>
        [Required]
        public Guid BlogId { get; set; }

        /// <summary>
        /// Gets/sets the category.
        /// </summary>
        [Required]
        public Taxonomy Category { get; set; }

        /// <summary>
        /// Gets/sets the optional primary image.
        /// </summary>
        public ImageField PrimaryImage { get; set; } = new ImageField();

        /// <summary>
        /// Gets/sets the optional excerpt.
        /// </summary>
        public string Excerpt { get; set; }

        /// <summary>
        /// Gets/sets the optional redirect.
        /// </summary>
        [StringLength(256)]
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Gets/sets the redirect type.
        /// </summary>
        public RedirectType RedirectType { get; set; }

        /// <summary>
        /// Gets/sets the available tags.
        /// </summary>
        public IList<Taxonomy> Tags { get; set; } = new List<Taxonomy>();

        /// <summary>
        /// Gets/sets the available blocks.
        /// </summary>
        public IList<Extend.Block> Blocks { get; set; } = new List<Extend.Block>();

        /// <summary>
        /// Gets/sets if comments should be enabled.
        /// </summary>
        /// <value></value>
        public bool EnableComments { get; set; } = true;

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
        /// Checks if comments are open for this post.
        /// </summary>
        public bool IsCommentsOpen => EnableComments && Published.HasValue && (CloseCommentsAfterDays == 0 || Published.Value.AddDays(CloseCommentsAfterDays) > DateTime.Now);
    }
}
