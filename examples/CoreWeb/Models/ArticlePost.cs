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
    /// Basic post with main content in markdown.
    /// </summary>
    [PostType(Title = "Article")]
    public class ArticlePost : Post<ArticlePost>
    {
        /// <summary>
        /// Gets/sets the main content.
        /// </summary>
        [Region(Title = "Main content")]
        public MarkdownField Body { get; set; }

        /// <summary>
        /// Gets/sets the heading.
        /// </summary>
        [Region]
        public PageHeading Heading { get; set; }        
    }
}
