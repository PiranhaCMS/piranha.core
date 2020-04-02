/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using Piranha.Extend.Fields;

namespace Piranha.Extend.Blocks
{
    /// <summary>
    /// Single column quote block.
    /// </summary>
    [BlockType(Name = "Quote", Category = "Content", Icon = "fas fa-quote-right", Component = "quote-block")]
    public class QuoteBlock : Block, ISearchable
    {
        /// <summary>
        /// Gets/sets the text body.
        /// </summary>
        public TextField Body { get; set; }

        /// <summary>
        /// Gets the content that should be indexed for searching.
        /// </summary>
        public override string GetTitle()
        {
            if (Body?.Value != null)
            {
                return Body.Value;
            }
            return "Empty";
        }

        /// <summary>
        /// Gets the content that should be indexed for searching.
        /// </summary>
        public string GetIndexedContent()
        {
            return !string.IsNullOrEmpty(Body.Value) ? Body.Value : "";
        }
    }
}
