/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/tidyui/coreweb
 * 
 */

using CoreWebAngular.Models.Fields;
using Piranha.AttributeBuilder;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace CoreWebAngular.Models.Regions
{
    /// <summary>
    /// Simple region for a teaser.
    /// </summary>
    public class Teaser
    {
        /// <summary>
        /// Gets/sets the title.
        /// </summary>
        [Field(Options = FieldOption.HalfWidth)]
        public StringField Title { get; set; }

        /// <summary>
        /// Gets/sets the optional teaser image.
        /// </summary>
        [Field]
        public SizedImageField Image { get; set; }

        /// <summary>
        /// Gets/sets the body.
        /// </summary>
        [Field]
        public HtmlField Body { get; set; }
    }
}
