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
        private readonly JsonSerializerSettings _jsonSettings;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="cache">The currently configured cache</param>
        public DistributedCache(IDistributedCache cache)
        {
            _cache = cache;
            _jsonSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
        }

        /// <summary>
        /// Gets the model with the specified key from cache.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="key">The unique key</param>
        /// <returns>The cached model, null it wasn't found</returns>
        public T Get<T>(string key)
        {
            var json = _cache.GetString(key);

            if (!string.IsNullOrEmpty(json))
            {
                return JsonConvert.DeserializeObject<T>(json, _jsonSettings);
            }
            return default(T);
        }

        /// <summary>
        /// Sets the given model in the cache.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="key">The unique key</param>
        /// <param name="value">The model</param>
        public void Set<T>(string key, T value)
        {
            _cache.SetString(key, JsonConvert.SerializeObject(value, _jsonSettings));
        }

        /// <summary>
        /// Removes the model with the specified key from cache.
        /// </summary>
        /// <param name="key">The unique key</param>
        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
