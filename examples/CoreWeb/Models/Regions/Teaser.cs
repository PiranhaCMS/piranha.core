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
        [Field(Options = FieldOption.HalfWidth)]
        public StringField Title { get; set; }

        /// <summary>
        /// Gets/sets the subtitle.
        /// </summary>
        [Field(Options = FieldOption.HalfWidth)]
        public StringField SubTitle { get; set; }

        /// <summary>
        /// Gets/sets the body.
        /// </summary>
        [Field]
        public MarkdownField Body { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Teaser() {
            //
            // Note that fields in a region used as a collection item needs
            // to be initialized if they are to be seeded through code due to
            // a bug in the page repository.
            //
            Title = new StringField();
            SubTitle = new StringField();
            Body = new MarkdownField();
        }
    }
}
