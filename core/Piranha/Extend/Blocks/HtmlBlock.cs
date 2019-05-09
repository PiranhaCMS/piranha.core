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

namespace Piranha.Extend.Blocks
{
    /// <summary>
    /// Single column HTML block.
    /// </summary>
    [BlockType(Name = "Content", Category = "Content", Icon = "fas fa-paragraph", Component = "html-block")]
    public class HtmlBlock : Block
    {
        /// <summary>
        /// Gets/sets the HTML body.
        /// </summary>
        public HtmlField Body { get; set; }
    }
}
