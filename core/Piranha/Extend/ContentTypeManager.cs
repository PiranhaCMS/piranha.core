/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using System.Collections.Generic;

namespace Piranha.Extend
{
    public sealed class ContentTypeManager
    {
        private readonly List<AppContentType> contentTypes;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ContentTypeManager() {
            contentTypes = new List<AppContentType>();
        }

        /// <summary>
        /// Registers a new content type.
        /// </summary>
        /// <param name="id">The id that will be stored in the database</param>
        /// <param name="title">The display title of the content type</param>
        internal void Register<T>(string id, string title) {
            if (contentTypes.Exists(t => t.Type == typeof(T)))
                throw new ArgumentException($"Given type {typeof(T).FullName} is already registered as a content type.");
            
            contentTypes.Add(new AppContentType() {
                Id = id,
                Type = typeof(T),
                Title = title
            });
        }

        public string GetId(Type t) {
            foreach (var type in contentTypes) {
                if (type.Type.IsAssignableFrom(t))
                    return type.Id;
            }
            return null;
        }

        public string GetId<T>() {
            return GetId(typeof(T));
        }
    }
}
