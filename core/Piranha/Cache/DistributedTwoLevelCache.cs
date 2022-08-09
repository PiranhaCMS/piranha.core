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
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Piranha.Cache
{
    /// <summary>
    /// Two level distributed cache. The first level is a per application memory cache.
    /// The second level is the distributed cache
    /// </summary>
    public class DistributedTwoLevelCache : ICache
    {
        private readonly IMemoryCache _firstLevelCache;
        private readonly IDistributedCache _secondLevelCache;

        private readonly JsonSerializerSettings _jsonSettings;
        private readonly bool _clone;
        private string _cacheVersion;

        public const string CACHE_VERSION = "piranha_cache_version";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="firstLevelCache">First level memory cache</param>
        /// <param name="secondLevelCache">Second level distributed cache</param>
        public DistributedTwoLevelCache(IMemoryCache firstLevelCache, IDistributedCache secondLevelCache)
        {
            _firstLevelCache = firstLevelCache;
            _secondLevelCache = secondLevelCache;
            _jsonSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
        }

        /// <summary>
        /// Protected constructor
        /// </summary>
        /// <param name="firstLevelCache">First level memory cache</param>
        /// <param name="secondLevelCache">Second level distributed cache</param>
        /// <param name="clone"></param>
        protected DistributedTwoLevelCache(IMemoryCache firstLevelCache, IDistributedCache secondLevelCache, bool clone) : this(firstLevelCache, secondLevelCache)
        {
            _clone = clone;
        }

        /// <summary>
        /// Gets the model with the specified key from cache.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="key">The unique key</param>
        /// <returns>The cached model, null it wasn't found</returns>
        public T Get<T>(string key)
        {
            //Check if the item exists in the first level cache
            if (UseFirstLevelCache(key) && _firstLevelCache.TryGetValue<T>(key, out var obj))
            {
                if (!_clone)
                {
                    return obj;
                }

                if (obj != null)
                {
                    return Utils.DeepClone(obj);
                }

                return default(T);
            }

            //The item didn't exist in the first level
            //cache. Check in the second level cache
            var json = _secondLevelCache.GetString(key);

            if (!string.IsNullOrEmpty(json))
            {
                var ret = JsonConvert.DeserializeObject<T>(json, _jsonSettings);

                //Update first level cache
                SetUseFirstLevelCache(key, ret);

                return ret;
            }

            //No item was found in first or second level cache
            //Set first level cache to default(null)
            SetUseFirstLevelCache(key, default(T));

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
            _secondLevelCache.SetString(key, JsonConvert.SerializeObject(value, _jsonSettings));

            //Create a new cache version. This will "clear" the first
            //level cache for all instances
            CreateNewCacheVersion();

            //Cache it in the first level cache
            SetUseFirstLevelCache(key, value);
        }

        /// <summary>
        /// Removes the model with the specified key from cache.
        /// </summary>
        /// <param name="key">The unique key</param>
        public void Remove(string key)
        {
            _secondLevelCache.Remove(key);

            //Create a new cache version. This will "clear" the first
            //level cache for all instances
            CreateNewCacheVersion();

            SetUseFirstLevelCache(key, null);
        }

        /// <summary>
        /// Invalidates first level cache
        /// </summary>
        public void InvalidateFirstLevelCache()
        {
            //Will cause that all first level cache items
            //are invalid
            _cacheVersion = null;
        }

        private string CacheVersion
        {
            get
            {
                //If cacheversion is null this is the first time we check it for this request
                //Then we need to get the current version from the distributed cache
                //If no current version exists in the distributed cache we need to create it
                if (_cacheVersion == null)
                {
                    _cacheVersion = _secondLevelCache.GetString(CACHE_VERSION);
                    if (_cacheVersion == null)
                    {
                        CreateNewCacheVersion();
                    }
                }
                return _cacheVersion;
            }
        }

        /// <summary>
        /// Creates a new cache version This will "clear" the first
        /// level cache.
        /// </summary>
        private void CreateNewCacheVersion()
        {
            _cacheVersion = Guid.NewGuid().ToString("N");
            _secondLevelCache.SetString(CACHE_VERSION, _cacheVersion);
        }

        /// <summary>
        /// Checks if first level cache should be used
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        private bool UseFirstLevelCache(string key)
        {
            _firstLevelCache.TryGetValue<string>(GetFirstLevelCacheVersionKey(key), out var obj);
            return obj != null && obj == "1";
        }

        /// <summary>
        /// Sets that we should use the first level cache for the specified key
        /// </summary>
        /// <param name="key">key</param>
        private void SetUseFirstLevelCache(string key, object obj)
        {
            _firstLevelCache.Set(key, obj, new MemoryCacheEntryOptions() );
            _firstLevelCache.Set(GetFirstLevelCacheVersionKey(key), "1");
        }

        /// <summary>
        /// Gets the first level cache key version
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetFirstLevelCacheVersionKey(string key)
        {
            return key + "_" + CacheVersion;
        }

    }


}
