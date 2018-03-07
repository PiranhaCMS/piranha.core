/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Linq;
using System.Reflection;

namespace Piranha.Extend
{
    public sealed class AppFieldList : AppDataList<IField, AppField>
    {
        /// <summary>
        /// Gets a single item by its shorthand name.
        /// </summary>
        /// <param name="shorthand">The shorthand name</param>
        /// <returns>The item, null if not found</returns>
        public AppField GetByShorthand(string shorthand)
        {
            return items.FirstOrDefault(i => i.Shorthand == shorthand);
        }

        /// <summary>
        /// Performs additional processing on the item before
        /// adding it to the collection.
        /// </summary>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <param name="item">The item</param>
        /// <returns>The processed item</returns>
        protected override AppField OnRegister<TValue>(AppField item)
        {
            var attr = typeof(TValue).GetTypeInfo().GetCustomAttribute<FieldAttribute>();
            if (attr == null)
            {
                return item;
            }

            item.Name = attr.Name;
            item.Shorthand = attr.Shorthand;

            return item;
        }
    }
}
