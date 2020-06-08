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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Piranha.Extend.Fields
{
    /// <summary>
    /// Generic select field.
    /// </summary>
    [FieldType(Name = "DataSelect", Shorthand = "DataSelect", Component = "data-select-field")]
    public class DataSelectField<T> : DataSelectFieldBase where T : class
    {
        /// <summary>
        /// Gets the currently selected value.
        /// </summary>
        public T Value { get; set; }

        public async Task Init(IServiceProvider services)
        {
            if (string.IsNullOrWhiteSpace(Id)) return;

            var get = typeof(T).GetMethod("GetById", BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static);

            if (get != null)
            {
                // Now inject any other parameters
                using (var scope = services.CreateScope())
                {
                    var param = new List<object>();

                    // First add the current id to the params
                    param.Add(Id);

                    foreach (var p in get.GetParameters().Skip(1))
                    {
                        param.Add(scope.ServiceProvider.GetService(p.ParameterType));
                    }

                    // Check for async
                    if (typeof(Task<T>).IsAssignableFrom(get.ReturnType))
                    {
                        Value = await ((Task<T>)get.Invoke(null, param.ToArray())).ConfigureAwait(false);
                    }
                    else
                    {
                        await Task.Run(() =>
                        {
                            Value = (T)get.Invoke(null, param.ToArray());
                        });
                    }
                }
            }
        }

        public async Task InitManager(IServiceProvider services)
        {
            var get = typeof(T).GetMethod("GetList", BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static);

            if (get != null)
            {
                using (var scope = services.CreateScope())
                {
                    var param = new List<object>();

                    foreach (var p in get.GetParameters())
                    {
                        param.Add(scope.ServiceProvider.GetService(p.ParameterType));
                    }

                    // Check for async
                    if (typeof(Task<IEnumerable<DataSelectFieldItem>>).IsAssignableFrom(get.ReturnType))
                    {
                        Items = (await ((Task<IEnumerable<DataSelectFieldItem>>)get.Invoke(null, param.ToArray())).ConfigureAwait(false)).ToArray();
                    }
                    else
                    {
                        await Task.Run(() =>
                        {
                            Items = ((IEnumerable<DataSelectFieldItem>)get.Invoke(null, param.ToArray())).ToArray();
                        });
                    }
                }
            }
        }

        public override string GetTitle()
        {
            if (Value != null)
            {
                return Value.ToString();
            }
            return "Not item selected";
        }
    }

    public class DataSelectFieldItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public abstract class DataSelectFieldBase : IField
    {
        /// <summary>
        /// Gets/sets the id of the currently selected value.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets/sets the available items to selected from, this is
        /// only used in the manager.
        /// </summary>
        public IEnumerable<DataSelectFieldItem> Items { get; set; }

        /// <summary>
        /// Gets the list item title if this field is used in
        /// a collection regions.
        /// </summary>
        public abstract string GetTitle();
    }
}