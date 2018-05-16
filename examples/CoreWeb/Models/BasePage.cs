/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/tidyui/coreweb
 * 
 */

using Piranha.AttributeBuilder;
using Piranha.Models;
using Piranha.Extend.Fields;

namespace CoreWeb.Models
{
    /// <summary>
    /// Base page for the example site.
    /// </summary>
    public abstract class BasePage<T> : Page<T> where T : BasePage<T>
    {
        /// <summary>
        /// Gets/sets the page heading.
        /// </summary>
        [Region(SortOrder = 0)]
        public Regions.PageHeading Heading { get; set; }
    }
}
