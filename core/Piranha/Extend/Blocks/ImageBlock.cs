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
    [BlockType(Name = "Image", Category = "Media", Icon = "glyphicon glyphicon-picture")]
    public class ImageBlock : Block
    {
        public ImageField Body { get; set; }
    }
}
