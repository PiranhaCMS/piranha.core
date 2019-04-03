/*
 * Copyright (c) 2017-2018 Håkan Edling
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

namespace MvcWeb.Models.Regions
{
    /// <summary>
    /// Simple hero region.
    /// </summary>
    public class Hero
    {
        /// <summary>
        /// Gets/sets the optional subtitle.
        /// </summary>
        [Field(Options = FieldOption.HalfWidth, Placeholder = "Optional subtitle")]
        public StringField Subtitle { get; set; }

        /// <summary>
        /// Gets/sets the optional primary image.
        /// </summary>
        [Field(Title = "Primary Image", Options = FieldOption.HalfWidth, Placeholder = "Optional hero background image")]
        public ImageField PrimaryImage { get; set; }

        [Field(Title = "Text value", Placeholder = "This is a text value")]
        public TextField TextValue { get; set; }

        [Field(Title = "Checkbox value", Placeholder = "Click me to make me happy!!!")]
        public CheckBoxField CheckBoxValue { get; set; }

        [Field(Title = "Number value", Options = FieldOption.HalfWidth, Placeholder = "Just give me a random number")]
        public NumberField NumberValue { get; set; }

        [Field(Title = "Date value", Options = FieldOption.HalfWidth, Placeholder = "Please enter your birthdate")]
        public DateField DateValue { get; set; }

        /// <summary>
        /// Gets/sets the optional ingress.
        /// </summary>
        [Field()]
        public HtmlField Ingress { get; set; }
    }
}
