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
using Piranha.Models;

namespace Piranha.Extend.Blocks
{
    /// <summary>
    /// Single column quote block.
    /// </summary>
    [BlockGroupType(Name = "Columns", Category = "Content", Icon = "fas fa-columns", Display = BlockDisplayMode.Horizontal)]
    public class ColumnBlock : BlockGroup, ISearchable
    {
        /// <summary>
        /// Gets the content that should be indexed for searching.
        /// </summary>
        public string GetIndexedContent()
        {
            var content = new StringBuilder();

            foreach (var item in Items)
            {
                if (item is ISearchable searchItem)
                {
                    var value = searchItem.GetIndexedContent();

                    if (!string.IsNullOrEmpty(value))
                    {
                        content.AppendLine(value);
                    }
                }
            }
            return content.ToString();
        }
    }
}
