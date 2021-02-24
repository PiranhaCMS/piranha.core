/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;

namespace Piranha.Models
{
    /// <summary>
    /// The validation state of a comment.
    /// Note: This replace the IsApproved bool of Comments.
    /// Pending represent unvalidated comments,
    /// Approved and Rejected represents validation result.
    /// 
    /// </summary>
    [Serializable]
    public enum CommentStatus
    {
        /// <summary>
        /// Comment is pending approval.
        /// </summary>
        Pending,
        /// <summary>
        /// Commend is approved.
        /// </summary>
        Approved,
        /// <summary>
        /// Comment is rejected.
        /// </summary>
        Rejected,
    }
}
