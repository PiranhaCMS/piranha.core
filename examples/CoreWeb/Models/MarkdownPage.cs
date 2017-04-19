/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Piranha.AttributeBuilder;
using Piranha.Models;
using Piranha.Extend.Fields;

namespace CoreWeb.Models
{
    [PageType(Title = "Markdown page")]
    public class MarkdownPage : Page<MarkdownPage>
    {
        public class PageHeading 
        {
            [Field(Title = "Primary image")]
            public ImageField PrimaryImage { get; set; }

            [Field]
            public TextField Ingress { get; set; }
        }

        [Region]
        public MarkdownField Body { get; set; }

        [Region]
        public PageHeading Heading { get; set; }
    }
}
