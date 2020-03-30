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
        /// </summary>
        public bool IsApproved { get; set; } = true;

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