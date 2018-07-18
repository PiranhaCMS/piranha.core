/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using CoreWebAngular.Models.Fields;
using Piranha.Extend;
using Piranha.Extend.Fields;

namespace CoreWebAngular.Models.Blocks
{
    /// <summary>
    /// Image block.
    /// </summary>
    [BlockType(Name = "SizedImage", Category = "Media", Icon = "fas fa-image")]
    public class SizedImageBlock : Block
    {
        /// <summary>
        /// Gets/sets the image body.
        /// </summary>
        public SizedImageField Body { get; set; }
    }
}
