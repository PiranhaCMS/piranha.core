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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Piranha.Runtime
{
    public abstract class AppDataList<T, TItem> : IEnumerable<TItem> where TItem : AppDataItem
    {
        /// <summary>
        /// The items collection.
        /// </summary>
        protected readonly List<TItem> _items = new List<TItem>();

        /// <summary>
        /// Registers a new item.
        /// </summary>
        /// <typeparam name="TValue">The value type</typeparam>
        public virtual void Register<TValue>() where TValue : T
        {
            var type = typeof(TValue);

            //
            // Make sure we don't register the same type multiple times.
            //
            if (_items.Where(i => i.Type == type).Count() == 0)
            {
                var item = Activator.CreateInstance<TItem>();

                item.Type = type;
                item.TypeName = type.FullName;

                _items.Add(OnRegister<TValue>(item));
            }
        }

        /// <summary>
        /// Unregisters a previously registered item.
        /// </summary>
        /// <typeparam name="TValue">The value type</typeparam>
        public virtual void UnRegister<TValue>() where TValue : T
        {
            var item = _items.SingleOrDefault(i => i.Type == typeof(TValue));
            if (item != null)
            {
                _items.Remove(item);
            }
        }

        /// <summary>
        /// Gets a single item by its type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The item, null if not found</returns>
        public virtual TItem GetByType(Type type)
        {
            return _items.SingleOrDefault(i => i.Type == type);
        }

        /// <summary>
        /// Gets a single item by its full type name.
        /// </summary>
        /// <param name="typeName">The type name</param>
        /// <returns>The item, null if not found</returns>
        public virtual TItem GetByType(string typeName)
        {
            return _items.SingleOrDefault(i => i.TypeName == typeName);
        }

        /// <summary>
        /// Gets the generic enumerator for the items.
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<TItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator for the items.
        /// </summary>
        /// <returns>The enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <summary>
        /// Performs additional processing on the item before
        /// adding it to the collection.
        /// </summary>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <param name="item">The item</param>
        /// <returns>The processed item</returns>
        protected virtual TItem OnRegister<TValue>(TItem item) where TValue : T
        {
            return item;
        }
    }
}
