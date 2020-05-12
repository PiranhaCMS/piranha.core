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

namespace Piranha.Manager.Models.Content
{
    /// <summary>
    /// Edit model for a region.
    /// </summary>
    public class RegionModel
    {
        /// <summary>
        /// Gets/sets the available items. A region collection can have several items,
        /// a regular region will only have one item in the collection.
        /// </summary>
        public IList<RegionItemModel> Items { get; set; } = new List<RegionItemModel>();

        /// <summary>
        /// Gets/sets the meta information.
        /// </summary>
        public RegionMeta Meta { get; set; }
    }
}