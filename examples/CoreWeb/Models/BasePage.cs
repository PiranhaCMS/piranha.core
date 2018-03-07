/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/tidyui/coreweb
 * 
 */

using CoreWeb.Models.Regions;
using Piranha.AttributeBuilder;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace CoreWeb.Models
{
    /// <summary>
    /// Base page for the example site.
    /// </summary>
    public abstract class BasePage<T> : Page<T> where T : BasePage<T>
    {
        /// <summary>
        /// Gets/sets the main content.
        /// </summary>
        [Region(Title = "Main content", SortOrder = 0)]
        public MarkdownField Body { get; set; }

        /// <summary>
        /// Gets/sets the page heading.
        /// </summary>
        [Region(SortOrder = 1)]
        public PageHeading Heading { get; set; }
    }
}
