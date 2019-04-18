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
    /// Audio block.
    /// </summary>
    [BlockType(Name = "Audio", Category = "Media", Icon = "fas fa-headphones")]
    public class AudioBlock : Block
    {
        /// <summary>
        /// Gets/sets the Audio body.
        /// </summary>
        public AudioField Body { get; set; }
    }
}
