/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/tidyui/coreweb
 *
 */

using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace RazorWeb.Models.Regions
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
        /// Gets/sets the subtitle.
        /// </summary>
        [Field(Options = FieldOption.HalfWidth)]
        public StringField SubTitle { get; set; }

        /// <summary>
        /// Gets/sets the optional teaser image.
        /// </summary>
        [Field]
        public ImageField Image { get; set; }

        /// <summary>
        /// Gets/sets the main body.
        /// </summary>
        [Field]
        public HtmlField Body { get; set; }
    }
}
