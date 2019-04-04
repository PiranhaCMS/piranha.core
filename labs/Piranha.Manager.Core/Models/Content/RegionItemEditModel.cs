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

namespace Piranha.Manager.Models.Content
{
    /// <summary>
    /// Edit model for a region item.
    /// </summary>
    public class RegionItemEditModel
    {
        /// <summary>
        /// Gets/sets the available fields.
        /// </summary>
        public IList<FieldEditModel> Fields { get; set; } = new List<FieldEditModel>();
    }
}