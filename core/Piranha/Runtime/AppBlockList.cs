/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Piranha.Extend;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Piranha.Runtime
{
    public sealed class AppBlockList : AppDataList<Block, AppBlock>
    {
        /// <summary>
        /// Gets the block categories sorted in alphabetical order.
        /// </summary>
        /// <returns>The category types</returns>
        public IEnumerable<string> GetCategories()
        {
            return items
                .Select(i => i.Category)
                .Distinct().OrderBy(c => c)
                .ToArray();
        }

        /// <summary>
        /// Gets the blocks for the given category.
        /// </summary>
        /// <param name="category">The category</param>
        /// <returns>The block types</returns>
        public IEnumerable<AppBlock> GetByCategory(string category)
        {
            return items
                .Where(i => i.Category == category)
                .ToArray();
        }

        /// <summary>
        /// Performs additional processing on the item before
        /// adding it to the collection.
        /// </summary>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <param name="item">The item</param>
        /// <returns>The processed item</returns>
        protected override AppBlock OnRegister<TValue>(AppBlock item)
        {
            var attr = typeof(TValue).GetTypeInfo().GetCustomAttribute<BlockTypeAttribute>();
            if (attr != null)
            {
                item.Name = attr.Name;
                item.Category = attr.Category;
                item.Icon = attr.Icon;
            }
            return item;
        }
    }
}
