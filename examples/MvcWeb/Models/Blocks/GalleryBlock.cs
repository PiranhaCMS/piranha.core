/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using Piranha.Extend;
using Piranha.Extend.Blocks;
using Piranha.Extend.Fields;

namespace MvcWeb.Models.Blocks
{
    /// <summary>
    /// Single column quote block.
    /// </summary>
    [BlockGroupType(Name = "Gallery", Category = "Media", Icon = "fas fa-images")]
    [BlockItemType(Type = typeof(ImageBlock))]
    public class GalleryBlock : BlockGroup
    {
        public StringField Title { get; set; }
        public HtmlField Description { get; set; }
    }
}
