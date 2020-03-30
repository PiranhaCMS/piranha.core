/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Collections.Generic;
using System.Linq;
using Piranha.Models;

namespace Piranha.Runtime
{
    public sealed class ContentTypeList<T> : List<T> where T : ContentTypeBase
    {
        /// <summary>
        /// Initializes the model from the given list of types.
        /// </summary>
        /// <param name="types">The content types</param>
        public void Init(IEnumerable<T> types)
        {
            // Add the types
            foreach (var type in types)
            {
                Add(type);
            }

            // Register runtime hooks to update the collection
            App.Hooks.RegisterOnAfterSave<T>((model) =>
            {
                var old = this.FirstOrDefault(t => t.Id == model.Id);

                if (old != null)
                {
                    Remove(old);
                }
                Add(model);
            });
        }

        /// <summary>
        /// Gets the content type with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The content type</returns>
        public T GetById(string id)
        {
            return this.FirstOrDefault(t => t.Id == id);
        }
    }
}
