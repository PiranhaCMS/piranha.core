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

namespace Piranha.Manager.Models
{
    /// <summary>
    /// Block list model.
    /// </summary>
    public class BlockListModel
    {
        /// <summary>
        /// A block category in the list.
        /// </summary>
        public class ListCategory
        {
            /// <summary>
            /// Gets/sets the name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets/sets the available block types for the category.
            /// </summary>
            public IList<ListItem> Items { get; set; } = new List<ListItem>();
        }

        /// <summary>
        /// A block type item in the list.
        /// </summary>
        public class ListItem
        {
            /// <summary>
            /// Gets/sets the name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets/sets the css icon.
            /// </summary>
            public string Icon { get; set; }

            /// <summary>
            /// Gets/sets the block type.
            /// </summary>
            public string Type { get; set; }
        }

        /// <summary>
        /// Gets/sets the available block categories.
        /// </summary>
        public IList<ListCategory> Categories { get; set; } = new List<ListCategory>();

        /// <summary>
        /// Gets/sets the total number of block types.
        /// </summary>
        public int TypeCount { get; set; }
    }
}