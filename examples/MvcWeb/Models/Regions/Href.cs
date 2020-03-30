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

namespace MvcWeb.Models.Regions
{
    /// <summary>
    /// Simple link region.
    /// </summary>
    public class Href
    {
        [Field(Title = "Button Text", Options = FieldOption.HalfWidth)]
        public StringField ButtonText { get; set; }

        [Field(Title = "Button Link", Options = FieldOption.HalfWidth)]
        public PageField ButtonLink { get; set; }
    }
}
