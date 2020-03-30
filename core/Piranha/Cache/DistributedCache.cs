/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Piranha.Cache
{
    /// <summary>
    /// Simple in memory cache.
    /// </summary>
    public class DistributedCache : ICache
    {
        private readonly IDistributedCache _cache;
        private readonly Dictionary<Type, bool> _types = new Dictionary<Type, bool>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="cache">The currently configured cache</param>
        public DistributedCache(IDistributedCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Gets the model with the specified key from cache.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="key">The unique key</param>
        /// <returns>The cached model, null it wasn't found</returns>
        public T Get<T>(string key)
        {
            return Deserialize<T>(_cache.Get(key));
        }

        /// <summary>
        /// Sets the given model in the cache.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="key">The unique key</param>
        /// <param name="value">The model</param>
        public void Set<T>(string key, T value)
        {
            _cache.Set(key, Serialize(value));
        }

        /// <summary>
        /// Removes the model with the specified key from cache.
        /// </summary>
        /// <param name="key">The unique key</param>
        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        /// <summary>
        /// Serializes the given object to a byte array.
        /// </summary>
        /// <param name="obj">The object</param>
        /// <returns>The serialized byte array</returns>
        private byte[] Serialize(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                if (IsSerializable(obj.GetType()))
                {
                    formatter.Serialize(stream, obj);
                    return stream.ToArray();
                }

                // First, serialize the object to JSON.
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                };
                var json = JsonConvert.SerializeObject(obj, settings);

                // Next lets convert the json to a byte array
                formatter.Serialize(stream, json);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Deserializes the byte array to an object.
        /// </summary>
        /// <param name="bytes">The byte array</param>
        /// <typeparam name="T">The object type</typeparam>
        /// <returns>The deserialized object</returns>
        private T Deserialize<T>(byte[] bytes)
        {
            if (bytes == null)
            {
                return default(T);
            }

            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream(bytes))
            {
                if (IsSerializable(typeof(T)))
                {
                    return (T)formatter.Deserialize(stream);
                }

                // First lets decode the byte array into a string
                var json = (string)formatter.Deserialize(stream);

                // Next deserialize the json into an object
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                };
                return JsonConvert.DeserializeObject<T>(json, settings);
            }
        }

        private bool IsSerializable(Type type)
        {
            if (_types.TryGetValue(type, out var serializable))
            {
                return serializable;
            }

            var attr = type.GetCustomAttribute<SerializableAttribute>();
            _types[type] = attr != null;

            return attr != null;
        }
    }
}
