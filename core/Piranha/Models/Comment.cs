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
using System.ComponentModel.DataAnnotations;

namespace Piranha.Models
{
    [Serializable]
    public sealed class Comment
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the id of the content this comment
        /// is in response to.
        /// </summary>
        public Guid ContentId { get; set; }

        /// <summary>
        /// Gets/sets the optional user id.
        /// </summary>
        [StringLength(128)]
        public string UserId { get; set; }

        /// <summary>
        /// Gets/sets the author name.
        /// </summary>
        [Required]
        [StringLength(128)]
        public string Author { get; set; }

        /// <summary>
        /// Gets/sets the email address.
        /// </summary>
        [Required, EmailAddress]
        [StringLength(128)]
        public string Email { get; set; }

        /// <summary>
        /// Gets/sets the optional website URL.
        /// </summary>
        [StringLength(256)]
        public string Url { get; set; }

        /// <summary>
        /// Gets/sets the IP Address. This is not stored
        /// but can be used for comment validation.
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// Gets/sets the User Agent. This is not stored but
        /// can be used for comment validation.
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Comments are approved by default unless you use some kind of comment
        /// validation mechanism.
        /// Note on obsolete:
        /// IsApproved=True corresponds to Status=CommentStatus.Approved
        /// IsApproved=False corresponds to Status=CommentStatus.Rejected
        /// Submitted comments with config set to NOT approve directly will result in
        /// Status=CommentStatus.Pending
        /// In other words: there is no way to deal with pending comments using IsApproved
        /// </summary>
        [Obsolete("IsApproved is obsolete and has been replaced with CommentStatus", false)]
        public bool IsApproved 
        {
            get => Status == CommentStatus.Approved;
            set => Status = value ? CommentStatus.Approved : CommentStatus.Rejected;
        }

        /// <summary>
        /// Gets/sets the comment status. 
        /// Models.CommentStatus.Approved is equivalent to <see cref="Models.Comment.IsApproved"/> = true.
        /// </summary>
        [Required]
        public CommentStatus Status { get; set; } = CommentStatus.Approved;

        /// <summary>
        /// Gets/sets the optional reason for current status
        /// </summary>
        public string StatusReason { get; set; }

        /// <summary>
        /// Gets/sets the comment body.
        /// </summary>
        [Required]
        public string Body { get; set; }

        /// <summary>
        /// Gets/sets the created date.
        /// </summary>
        [Required]
        public DateTime Created { get; set; }
    }
}