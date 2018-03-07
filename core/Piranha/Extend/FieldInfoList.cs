/*
 * Copyright (c) 2016-2017 Håkan Edling
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
using System.Reflection;

namespace Piranha.Extend
{
    public class FieldInfoList : IEnumerable<FieldInfo>
    {
        #region Members
        /// <summary>
        /// The private collection.
        /// </summary>
        private readonly List<FieldInfo> _items = new List<FieldInfo>();
        #endregion

        /// <summary>
        /// Registers a new field.
        /// </summary>
        /// <typeparam name="T">The field type</typeparam>
        public void Register<T>() where T : IField
        {
            var type = typeof(T);

            //
            // Make sure we don't register the same type multiple times.
            //
            if (_items.Count(i => i.Type == type) != 0)
            {
                return;
            }

            var field = new FieldInfo
            {
                CLRType = type.FullName,
                Type = type
            };

            var attr = type.GetTypeInfo().GetCustomAttribute<FieldAttribute>();
            if (attr != null)
            {
                field.Name = attr.Name;
                field.Shorthand = attr.Shorthand;
            }

            _items.Add(field);
        }

        /// <summary>
        /// Unregisters a previously registered field.
        /// </summary>
        /// <typeparam name="T">The field type</typeparam>
        public void UnRegister<T>() where T : IField
        {
            var item = _items.SingleOrDefault(i => i.Type == typeof(T));
            if (item != null)
            {
                _items.Remove(item);
            }
        }

        /// <summary>
        /// Gets a single field by its type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The field, null if not found</returns>
        public FieldInfo GetByType(Type type)
        {
            return _items.SingleOrDefault(i => i.Type == type);
        }

        /// <summary>
        /// Gets a single field by its full type name.
        /// </summary>
        /// <param name="typeName">The type name</param>
        /// <returns>The field, null if not found</returns>
        public FieldInfo GetByType(string typeName)
        {
            return _items.SingleOrDefault(i => i.CLRType == typeName);
        }

        /// <summary>
        /// Gets a single field by its shorthand name.
        /// </summary>
        /// <param name="shorthand">The shorthand name</param>
        /// <returns>The field, null if not found</returns>
        public FieldInfo GetByShorthand(string shorthand)
        {
            return _items.SingleOrDefault(i => i.Shorthand == shorthand);
        }

        /// <summary>
        /// Gets the generic enumerator for the items.
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<FieldInfo> GetEnumerator()
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
    }
}
