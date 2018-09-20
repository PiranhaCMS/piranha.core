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
using Piranha.Extend.Fields;
using Piranha.Models;

namespace CoreWeb.Models.Regions
{
    /// <summary>
    /// Simple region for a teaser.
    /// </summary>
    public class Teaser
    {
        /// <summary>
        /// Gets/sets the title.
        /// </summary>
        [Field(Options = FieldOption.HalfWidth, Placeholder = "Mandatory title")]
        public StringField Title { get; set; }

        /// <summary>
        /// Gets/sets the optional teaser image.
        /// </summary>
        [Field]
        public ImageField Image { get; set; }

        /// <summary>
        /// Gets/sets the body.
        /// </summary>
        [Field]
        public HtmlField Body { get; set; }
    }
}
