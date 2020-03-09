/*
 * Copyright (c) 2020 Piranha CMS
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Piranha.Models
{
    /// <summary>
    /// Base class for all content.
    /// </summary>
    public abstract class Content
    {
        /// <summary>
        /// Get/sets the unique id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the type id.
        /// </summary>
        public string TypeId { get; set; }

        /// <summary>
        /// Gets/sets the main title.
        /// </summary>
        [StringLength(128)]
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets if comments should be enabled.
        /// </summary>
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
        /// Gets/sets the permissions needed to access the content.
        /// </summary>
        public IList<string> Permissions { get; set; } = new List<string>();

        /// <summary>
        /// Gets/sets the created date.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets/sets the last modification date.
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Gets/sets the published date.
        /// </summary>
        public DateTime? Published { get; set; }
    }
}