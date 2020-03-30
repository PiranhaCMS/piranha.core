/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace MvcWeb.Models.Regions
{
    /// <summary>
    /// Test Field with all field types.
    /// </summary>
    public class AllFields
    {
        [Field(Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        public AudioField Audio { get; set; }

        [Field(Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        public CheckBoxField CheckBox { get; set; }

        [Field(Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        public DateField Date { get; set; }

        [Field(Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        public HtmlField Html { get; set; }

        [Field(Options = FieldOption.HalfWidth, Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        public DocumentField Document { get; set; }

        [Field(Options = FieldOption.HalfWidth, Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        public ImageField Image { get; set; }

        [Field(Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        public MediaField Media { get; set; }

        [Field(Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        [FieldDescription("Duis mollis, est non <strong>commodo luctus</strong>, nisi erat porttitor ligula, eget lacinia odio sem nec elit. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.")]
        public VideoField Video { get; set; }

        [Field(Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        public MarkdownField Markdown { get; set; }

        [Field(Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        public NumberField Number { get; set; }

        [Field(Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        public PageField Page { get; set; }

        [Field(Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        public PostField Post { get; set; }

        [Field(Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        public StringField String { get; set; }

        [Field(Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        public TextField Text { get; set; }
    }
}
