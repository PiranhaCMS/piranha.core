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
    /// <summary>
    /// Single column text block.
    /// </summary>
    [BlockType(Name = "Text", Category = "Content", Icon = "fas fa-font", Component = "text-block")]
    public class TextBlock : Block
    {
        /// <summary>
        /// Gets/sets the text body.
        /// </summary>
        public TextField Body { get; set; }
    }
}
