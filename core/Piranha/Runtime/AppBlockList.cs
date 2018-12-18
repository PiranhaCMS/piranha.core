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
            return _items
                .Select(i => i.Category)
                .Distinct().OrderBy(c => c)
                .ToArray();
        }

        /// <summary>
        /// Gets the blocks for the given category.
        /// </summary>
        /// <param name="category">The category</param>
        /// <returns>The block types</returns>
        public IEnumerable<AppBlock> GetByCategory(string category, bool includeGroups = true)
        {
            var query = _items
                .Where(i => i.Category == category);

            if (!includeGroups)
            {
                query = query
                    .Where(i => !typeof(Extend.BlockGroup).IsAssignableFrom(i.Type));
            }
            return query.ToArray();
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
                item.IsUnlisted = attr.IsUnlisted;

                if (attr is BlockGroupTypeAttribute)
                {
                    item.UseCustomView = ((BlockGroupTypeAttribute)attr).UseCustomView;
                }
            }

            var itemAttrs = typeof(TValue).GetTypeInfo().GetCustomAttributes(typeof(BlockItemTypeAttribute));
            foreach (var itemAttr in itemAttrs)
            {
                var itemType = ((BlockItemTypeAttribute)itemAttr).Type;

                // Block groups should not contain items that are
                // other block groups.
                if (!typeof(BlockGroup).IsAssignableFrom(itemType))
                {
                    item.ItemTypes.Add(itemType);
                }
            }
            return item;
        }
    }
}
