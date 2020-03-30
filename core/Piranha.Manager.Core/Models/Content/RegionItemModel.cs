/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Collections.Generic;

namespace Piranha.Manager.Models.Content
{
    /// <summary>
    /// Edit model for a region item.
    /// </summary>
    public class RegionItemModel
    {
        /// <summary>
        /// Gets/sets the unique client id.
        /// </summary>
        public string Uid { get; set; } = "uid-" + Math.Abs(Guid.NewGuid().GetHashCode()).ToString();

        /// <summary>
        /// Gets/sets the title if used in a list.
        /// </summary>
        public string Title { get; set; } = "...";

        /// <summary>
        /// Gets/sets the available fields.
        /// </summary>
        public IList<FieldModel> Fields { get; set; } = new List<FieldModel>();
    }
}