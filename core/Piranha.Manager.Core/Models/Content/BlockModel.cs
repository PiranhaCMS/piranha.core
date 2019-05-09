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
    public abstract class BlockModel
    {
        /// <summary>
        /// Gets/sets the meta information.
        /// </summary>
        public BlockMeta Meta { get; set; }
    }
}