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
        /// Gets/sets if the comment has been approved. Comments are
        /// approved by default unless you use some kind of comment
        /// validation mechanism.
        /// </summary>
        public bool IsApproved { get; set; } = true;

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