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
    [BlockType(Name = "Html", Category = "Basic", Icon = "glyphicon glyphicon-font")]
    public class HtmlBlock : Block
    {
        public HtmlField Body { get; set; }
    }
}
