﻿/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Extend;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Piranha.Runtime
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
            return _items.FirstOrDefault(i => i.Shorthand == shorthand);
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
            var attr = typeof(TValue).GetTypeInfo().GetCustomAttribute<Extend.FieldTypeAttribute>();
            if (attr != null)
            {
                item.Name = attr.Name;
                item.Shorthand = attr.Shorthand;
                item.Component = !string.IsNullOrWhiteSpace(attr.Component) ? attr.Component : "missing-field";
                item.Init.InitMethod = Utils.GetMethod<TValue>("Init");
                item.Init.InitManagerMethod = Utils.GetMethod<TValue>("InitManager");
            }
            return item;
        }

        /// <summary>
        /// Registers a new select field and its associated serializer.
        /// </summary>
        public void RegisterSelect<TValue>() where TValue : struct
        {
            Register<Extend.Fields.SelectField<TValue>>();
            App.Serializers.Register<Extend.Fields.SelectField<TValue>>(
                new Extend.Serializers.SelectFieldSerializer<Extend.Fields.SelectField<TValue>>()
            );
        }

        /// <summary>
        /// Registers a new data select field and its associated serializer.
        /// </summary>
        /// <typeparam name="TValue">The value type</typeparam>
        public void RegisterDataSelect<TValue>() where TValue : class
        {
            Register<Extend.Fields.DataSelectField<TValue>>();
            App.Serializers.Register<Extend.Fields.DataSelectField<TValue>>(
                new Extend.Serializers.DataSelectFieldSerializer<Extend.Fields.DataSelectField<TValue>>()
            );
        }
    }
}
