/*
 * Copyright (c) 2017-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System.Collections.Generic;

namespace Piranha.Cache
{
    /// <summary>
    /// Simple in memory cache.
    /// </summary>
    public class MemCache : ICache
    {
        /// <summary>
        /// The private cache collection.
        /// </summary>
        private readonly IDictionary<string, object> _cache = new Dictionary<string, object>();

        /// <summary>
        /// Gets the model with the specified key from cache.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="key">The unique key</param>
        /// <returns>The cached model, null it wasn't found</returns>
        public T Get<T>(string key)
        {
            object value;

            if (_cache.TryGetValue(key, out value))
            {
                return (T)value;
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
            _cache[key] = value;
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
