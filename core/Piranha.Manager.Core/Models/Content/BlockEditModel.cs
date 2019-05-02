/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Extend;

namespace Piranha.Manager.Models.Content
{
    /// <summary>
    /// Edit model for blocks.
    /// </summary>
    public class BlockEditModel
    {
        /// <summary>
        /// Gets/sets if the block should be active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets/sets the block model.
        /// </summary>
        /// <value></value>
        public Block Model { get; set; }

        /// <summary>
        /// Gets/sets the meta information.
        /// </summary>
        public ContentMeta Meta { get; set; }
    }
}