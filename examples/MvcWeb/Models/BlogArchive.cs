/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/tidyui/coreweb
 *
 */

using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Models;

namespace MvcWeb.Models
{
    /// <summary>
    /// Basic blog page.
    /// </summary>
    [PageType(Title = "Blog archive", UseBlocks = false, IsArchive = true)]
    public class BlogArchive : Page<BlogArchive>
    {
        /// <summary>
        /// Gets/sets the page header.
        /// </summary>
        [Region]
        public Regions.Hero Hero { get; set; }

        /// <summary>
        /// View model property for storing the current archive items.
        /// </summary>
        public PostArchive<DynamicPost> Archive { get; set; }
    }
}
