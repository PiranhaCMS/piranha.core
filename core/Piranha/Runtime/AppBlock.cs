/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

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
    }
}
