/*
 * Copyright (c) 2018-2019 Håkan Edling
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
    /// <summary>
    /// Connection between a post and a block.
    /// </summary>
    public sealed class PostBlock : IContentBlock
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the optional parent id if this page block
        /// is part of a group.
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Gets/sets the post id.
        /// </summary>
        public Guid PostId { get; set; }

        /// <summary>
        /// Gets/sets the block id.
        /// </summary>
        public Guid BlockId { get; set; }

        /// <summary>
        /// Gets/sets the zero based sort index.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets/sets the post containing the block.
        /// </summary>
        public Post Post { get; set; }

        /// <summary>
        /// Gets/sets the block data.
        /// </summary>
        public Block Block { get; set; }
    }
}
