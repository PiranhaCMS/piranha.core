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

namespace Piranha.Extend
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BlockTypeAttribute : Attribute
    {
        /// <summary>
        /// Gets/sets the display name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets/sets the block category.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets/set the icon css.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets/sets if the block type should only be listed
        /// where specified explicitly.
        /// </summary>
        public bool IsUnlisted { get; set; }
    }
}
