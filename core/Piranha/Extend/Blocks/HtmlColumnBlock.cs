/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Piranha.Extend.Fields;
using System;

namespace Piranha.Extend.Blocks
{
    [BlockType(Name = "Two Cols", Category = "Content", Icon = "fab fa-html5")]
    public class HtmlColumnBlock : Block
    {
        public HtmlField Column1 { get; set; }
        public HtmlField Column2 { get; set; }
    }
}
