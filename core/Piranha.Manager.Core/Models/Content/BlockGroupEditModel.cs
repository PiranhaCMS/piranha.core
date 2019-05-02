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
    public class BlockGroupEditModel : Block
    {
        /// <summary>
        /// Gets/sets the available child items in the group.
        /// </summary>
        public IList<BlockEditModel> Items { get; set; } = new List<BlockEditModel>();

        /// <summary>
        /// Gets/sets the available global group fields.
        /// </summary>
        public IList<FieldEditModel> Fields { get; set; } = new List<FieldEditModel>();
    }
}