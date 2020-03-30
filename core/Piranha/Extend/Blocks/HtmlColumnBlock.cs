/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Text;
using Piranha.Extend.Fields;

namespace Piranha.Extend.Blocks
{
    /// <summary>
    /// Two column HTML block.
    /// </summary>
    [BlockType(Name = "Two Cols", Category = "Content", Icon = "fab fa-html5", Component = "html-column-block", IsUnlisted = true)]
    public class HtmlColumnBlock : Block, ISearchable
    {
        /// <summary>
        /// Gets/sets the first column.
        /// </summary>
        public HtmlField Column1 { get; set; }

        /// <summary>
        /// Gets/sets the second column.
        /// </summary>
        public HtmlField Column2 { get; set; }

        /// <summary>
        /// Gets the content that should be indexed for searching.
        /// </summary>
        public string GetIndexedContent()
        {
            var content = new StringBuilder();

            // Add first column
            if (!string.IsNullOrEmpty(Column1.Value))
            {
                content.AppendLine(Column1.Value);
            }
            // Add second column
            if (!string.IsNullOrEmpty(Column2.Value))
            {
                content.AppendLine(Column2.Value);
            }
            return content.ToString();
        }
    }
}
