/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Models;

namespace Piranha.Manager.Models.Content
{
    /// <summary>
    /// Meta information for blocks.
    /// </summary>
    public class BlockMeta : ContentMeta
    {
        /// <summary>
        /// Gets/sets if this is a block group.
        /// </summary>
        public bool IsGroup { get; set; }

        /// <summary>
        /// Gets/sets if the block is collapsed.
        /// </summary>
        public bool isCollapsed { get; set; }

        /// <summary>
        /// Gets/sets if the block is readonly.
        /// </summary>
        public bool IsReadonly { get; set; }

        /// <summary>
        /// If the global header fields should be visible.
        /// </summary>
        public bool ShowHeader { get; set; } = true;

        /// <summary>
        /// Gets/sets the editor width.
        /// </summary>
        public string Width { get; set; }
    }
}