/*
 * Copyright (c) 2017 Håkan Edling
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
    public sealed class PostTag
    {
        /// <summary>
        /// Gets/sets the post id.
        /// </summary>
        public Guid PostId { get; set; }

        /// <summary>
        /// Gets/sets the tag id.
        /// </summary>
        public Guid TagId { get; set; }

        /// <summary>
        /// Gets/sets the post.
        /// </summary>
        public Post Post { get; set; }

        /// <summary>
        /// Gets/sets the tag.
        /// </summary>
        public Tag Tag { get; set; }
    }
}
