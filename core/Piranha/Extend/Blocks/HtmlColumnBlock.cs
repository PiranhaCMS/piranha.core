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
    /// Two column HTML block.
    /// </summary>
    [BlockType(Name = "Two Cols", Category = "Content", Icon = "fas fa-columns")]
    public class HtmlColumnBlock : Block
    {
        /// <summary>
        /// Gets/sets the first column.
        /// </summary>
        public HtmlField Column1 { get; set; }

        /// <summary>
        /// Gets/sets the second column.
        /// </summary>
        public HtmlField Column2 { get; set; }
    }
}
