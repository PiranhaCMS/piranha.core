/*
 * Copyright (c) 2019 Filip Jansson
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Piranha.Extend.Fields;

namespace Piranha.Extend.Blocks
{
    /// <summary>
    /// Video block.
    /// </summary>
    [BlockType(Name = "Video", Category = "Media", Icon = "fas fa-video")]
    public class VideoBlock : Block
    {
        /// <summary>
        /// Gets/sets the video body.
        /// </summary>
        public VideoField Body { get; set; }
    }
}
