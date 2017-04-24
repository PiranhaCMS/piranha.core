/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;

namespace Piranha.AttributeBuilder
{
    /// <summary>
    /// Attribute for marking a property as a region.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RegionAttribute : System.Attribute
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
        public string ListPlaceholder { get; set;}

        /// <summary>
        /// Gets/sets the maximum number of items if this is a collection.
        /// </summary>
        public int Max { get; set; }

        /// <summary>
        /// Gets/sets the minimum number of items if this is a collection.
        /// </summary>
        public int Min { get; set; }
    }
}
