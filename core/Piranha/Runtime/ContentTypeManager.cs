/*
 * Copyright (c) 2017-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Piranha.Extend;
using System;
using System.Collections.Generic;

namespace Piranha.Runtime
{
    public sealed class ContentTypeManager
    {
        private readonly List<AppContentType> _contentTypes;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ContentTypeManager()
        {
            _contentTypes = new List<AppContentType>();
        }

        /// <summary>
        /// Registers a new content type.
        /// </summary>
        /// <param name="id">The id that will be stored in the database</param>
        /// <param name="title">The display title of the content type</param>
        /// <param name="customEditor">If a custom editor should be added</param>
        internal void Register<T>(string id, string title, bool customEditor = false)
        {
            if (_contentTypes.Exists(t => t.Type == typeof(T)))
            {
                throw new ArgumentException($"Given type {typeof(T).FullName} is already registered as a content type.");
            }

            _contentTypes.Add(new AppContentType
            {
                Id = id,
                Type = typeof(T),
                Title = title,
                CustomEditor = customEditor
            });
        }

        public string GetId(Type t)
        {
            foreach (var type in _contentTypes)
            {
                if (type.Type.IsAssignableFrom(t))
                {
                    return type.Id;
                }
            }
            return null;
        }

        public string GetId<T>()
        {
            return GetId(typeof(T));
        }

        public AppContentType GetById(string id)
        {
            foreach (var type in _contentTypes)
            {
                if (type.Id == id)
                {
                    return type;
                }
            }
            return null;
        }
    }
}
