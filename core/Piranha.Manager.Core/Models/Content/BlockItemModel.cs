/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Collections.Generic;
using Piranha.Extend;

namespace Piranha.Manager.Models.Content
{
    /// <summary>
    /// Edit model for block groups.
    /// </summary>
    public class BlockItemModel : BlockModel
    {
        /// <summary>
        /// Gets/sets if the block should be active
        /// part of a group.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets/sets the block model.
        /// </summary>
        /// <value></value>
        public Block Model { get; set; }
    }
}