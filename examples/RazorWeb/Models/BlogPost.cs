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

namespace RazorWeb.Models
{
    /// <summary>
    /// Basic post with main content in markdown.
    /// </summary>
    [PostType(Title = "Blog post")]
    public class BlogPost : Post<BlogPost>
    {
        /// <summary>
        /// Gets/sets the heading.
        /// </summary>
        [Region()]
        public Regions.Hero Hero { get; set; }
    }
}
