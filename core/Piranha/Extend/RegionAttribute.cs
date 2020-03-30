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
using Piranha.Models;

namespace Piranha.Extend
{
    /// <summary>
    /// Attribute for marking a property as a region.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RegionAttribute : Attribute
    {
        /// <summary>
        /// Gets/sets the optional title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the field that will be used to generate the list
        /// item title if the region is used in a collection.
        /// </summary>
        public string ListTitle { get; set; }

        /// <summary>
        /// Gets/sets the placeholder title that will be used for new
        /// list items if the region is used in a collection.
        /// </summary>
        public string ListPlaceholder { get; set; }

        /// <summary>
        /// Gets/sets if list items should be expandable. If not, the
        /// content is placed directly in the title.
        /// </summary>
        public bool ListExpand { get; set; } = true;

        /// <summary>
        /// Gets/sets the optional sort order.
        /// </summary>
        public int SortOrder { get; set; } = Int32.MaxValue;

        /// <summary>
        /// Gets/sets the optional icon css.
        /// </summary>
        public string Icon { get; set; } = "fas fa-table";

        /// <summary>
        /// Gets/sets how the region should be displayed in
        /// the manager interface.
        /// </summary>
        public RegionDisplayMode Display { get; set; }
    }
}
