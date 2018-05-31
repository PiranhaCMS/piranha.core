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

namespace CoreWeb.Models.Regions
{
    public class SiteHeader
    {
        [Field]
        public StringField SiteTitle { get; set; }
        [Field]
        public StringField TagLine { get; set; }
    }
}
