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

namespace Piranha.Manager.Models
{
    public class CommentListModel
    {
        public class ListItem
        {
            public Guid Id { get; set; }
            public string Author { get; set; }
            public string AuthorImage { get; set; }
            public string Email { get; set; }
            public string Body { get; set; }
            public string ArticleTitle { get; set; }
            public string ArticleUrl { get; set; }
            public bool IsApproved { get; set; }
            public string Created { get; set; }
            internal DateTime CreatedDate { get; set; }
        }

        /// <summary>
        /// Gets/sets the optionally select content id.
        /// </summary>
        public Guid? ContentId { get; set; }

        /// <summary>
        /// Gets/sets the available comments.
        /// </summary>
        public IList<ListItem> Comments { get; set; } = new List<ListItem>();

        /// <summary>
        /// Gets/sets the optional status message from the last operation.
        /// </summary>
        public StatusMessage Status { get; set; }
    }
}