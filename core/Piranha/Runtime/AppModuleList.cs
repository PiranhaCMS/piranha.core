/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Extend;
using System;

namespace Piranha.Runtime
{
    public sealed class AppModuleList : AppDataList<IModule, AppModule>
    {
        /// <summary>
        /// Gets the module instance of the given type.
        /// </summary>
        /// <typeparam name="T">The module type</typeparam>
        /// <returns>The module instance</returns>
        public T Get<T>() where T : IModule
        {
            var module = GetByType(typeof(T));

            return (T)module?.Instance;
        }

        /// <summary>
        /// Performs additional processing on the item before
        /// adding it to the collection.
        /// </summary>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <param name="item">The item</param>
        /// <returns>The processed item</returns>
        protected override AppModule OnRegister<TValue>(AppModule item)
        {
            item.Instance = Activator.CreateInstance<TValue>();

            return item;
        }
    }
}
