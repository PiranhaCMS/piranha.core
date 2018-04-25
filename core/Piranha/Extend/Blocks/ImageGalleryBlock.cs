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
using System.Collections.Generic;

namespace Piranha.Extend.Blocks
{
    [BlockType(Name = "Gallery", Category = "Media", Icon = "fas fa-images")]
    public class ImageGalleryBlock : Block
    {
        public IList<ImageField> Items { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ImageGalleryBlock() {
            Items = new List<ImageField>();
        }
    }
}
