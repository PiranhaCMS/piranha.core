/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Collections.Generic;

namespace Piranha.Extend
{
    /// <summary>
    /// Base class for blocks that can contain other blocks.
    /// </summary>
    public abstract class BlockGroup : Block
    {
        /// <summary>
        /// Gets/sets the available blocks in this group.
        /// </summary>
        public IList<Block> Items { get; set; } = new List<Block>();
    }
}
