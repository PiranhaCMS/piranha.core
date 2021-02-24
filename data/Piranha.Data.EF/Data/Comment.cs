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

namespace Piranha.Data
{
    [Serializable]
    public class Comment
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the optional user id.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets/sets the author name.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets/sets the email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets/sets the optional website URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets/sets the comment status. 
        /// Models.CommentStatus.Approved is equivalent to <see cref="Models.Comment.IsApproved"/> = true.
        /// </summary>
        public Models.CommentStatus Status { get; set; } = Models.CommentStatus.Approved;

        /// <summary>
        /// Gets/sets the optional reason for current status
        /// </summary>
        public string StatusReason { get; set; }

        /// <summary>
        /// Gets/sets the comment body.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets/sets the created date.
        /// </summary>
        public DateTime Created { get; set; }
    }
}