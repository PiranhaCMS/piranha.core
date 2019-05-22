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
    /// Test Field with all field types.
    /// </summary>
    public class AllFields
    {
        [Field]
        public AudioField Audio { get; set; }
        [Field]
        public CheckBoxField CheckBox { get; set; }
        [Field]
        public DateField Date { get; set; }
        [Field]
        public HtmlField Html { get; set; }
        [Field]
        public DocumentField Document { get; set; }
        [Field]
        public ImageField Image { get; set; }
        [Field]
        public MediaField Media { get; set; }
        [Field]
        public VideoField Video { get; set; }
        [Field]
        public MarkdownField Markdown { get; set; }
        [Field]
        public NumberField Number { get; set; }
        [Field]
        public PageField Page { get; set; }
        [Field]
        public PostField Post { get; set; }
        [Field]
        public StringField String { get; set; }
        [Field]
        public TextField Text { get; set; }
    }
}
