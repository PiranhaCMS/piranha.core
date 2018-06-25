/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/tidyui/coreweb
 * 
 */

namespace Piranha.Extend.Blocks
{
    /// <summary>
    /// Simple region for some optional page heading information.
    /// </summary>
    [BlockType(Name = "Slider", Category = "Groups", Icon = "far fa-newspaper")]
    [BlockItemType(Type = typeof(HtmlBlock))]
    [BlockItemType(Type = typeof(HtmlColumnBlock))]

    public class SliderGroup : BlockGroup
    {
        public Fields.StringField MainTitle { get; set; }
        public Fields.ImageField MainImage { get; set; }
    }
}
