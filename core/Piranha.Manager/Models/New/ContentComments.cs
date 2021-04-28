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
    public sealed class ContentComments
    {
        /// <summary>
        /// Gets/sets if comments should be enabled for this content.
        /// </summary>
        public bool EnableComments { get; set; }

        /// <summary>
        /// Gets/sets after how many days after publish date comments
        /// should be closed. A value of 0 means never.
        /// </summary>
        public int CloseCommentsAfterDays { get; set; }

        /// <summary>
        /// Gets/sets the total comment count.
        /// </summary>
        public int CommentCount { get; set; }

        /// <summary>
        /// Gets/sets the number of pending comments.
        /// </summary>
        /// <value></value>
        public int PendingCommentCount { get; set; }
    }
}