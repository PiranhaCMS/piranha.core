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
    /// Basic blog page.
    /// </summary>
    [PageType(Title = "Blog Archive")]
    public class BlogArchive : BlogPage<BlogArchive>
    {
        /// <summary>
        /// Gets/sets the main content.
        /// </summary>
        [Region(Title = "Main content", SortOrder = 0)]
        public MarkdownField Body { get; set; }
    }
}
