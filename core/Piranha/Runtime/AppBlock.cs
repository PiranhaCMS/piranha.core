/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using System.Collections.Generic;

namespace Piranha.Runtime
{
    public sealed class AppBlock : AppDataItem
    {
        /// <summary>
        /// Gets/sets the display name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets/sets the category.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets/sets the block icon.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets/sets if the block type should only be listed
        /// where specified explicitly.
        /// </summary>
        public bool IsUnlisted { get; set; }

        /// <summary>
        /// Gets/sets if the block group should use a 
        /// custom view.
        /// </summary>
        public bool UseCustomView { get; set; }

        /// <summary>
        /// Gets/sets the specified item types.
        /// </summary>
        public IList<Type> ItemTypes { get; set; } = new List<Type>();
    }
}
