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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace Piranha.Tests
{
    public class DistributedTwoLevelCache
    {

        public DistributedTwoLevelCache() {
        }

        [Fact]
        public void SingleInstanceGetSetRemove()
        {
            var firstLevelCache = new MemoryCacheMock();
            var secondLevelCache = new DistributedCacheMock();
            var distributedCache = new Cache.DistributedTwoLevelCache(firstLevelCache, secondLevelCache);

            var key = "id";

            //Set item for first time
            //This will cause 2 hits on second level cache
            distributedCache.Set(key, "ver");
            Assert.Equal(2, secondLevelCache.Hits);

            //Get item from cache. The first level
            //cache will be used. No hits on the second level
            //cache
            Assert.Equal("ver", distributedCache.Get<string>(key));
            Assert.Equal(2, secondLevelCache.Hits);

            //Update the item
            //This should cause 2 hits on the second level
            //cache
            distributedCache.Set(key, "ver2");
            Assert.Equal(4, secondLevelCache.Hits);

            //Get item from cache again once
            //No second level cache hits
            Assert.Equal("ver2", distributedCache.Get<string>(key));
            Assert.Equal(4, secondLevelCache.Hits);

            //Get item from cache again twice
            //No second level cache hits
            Assert.Equal("ver2", distributedCache.Get<string>(key));
            Assert.Equal(4, secondLevelCache.Hits);

            //Remove item
            //This will cause 2 hits on second level cache
            distributedCache.Remove(key);
            Assert.Equal(6, secondLevelCache.Hits);

            //Get removed item from cache
            //No second level cache hits
            Assert.Null(distributedCache.Get<string>(key));
            Assert.Equal(6, secondLevelCache.Hits);

        }

        [Fact]
        public void TwoInstanceGetSetRemove()
        {
            var secondLevelCache = new DistributedCacheMock();
            var firstLevelCache1 = new MemoryCacheMock();
            var firstLevelCache2 = new MemoryCacheMock();
            var instance1 = new Cache.DistributedTwoLevelCache(firstLevelCache1, secondLevelCache);
            var instance2 = new Cache.DistributedTwoLevelCache(firstLevelCache2, secondLevelCache);

            //Test item key
            var key = "id";

            //Set item from instance 1
            //2 hits on second level cache
            instance1.Set(key, "ver");
            Assert.Equal(2, secondLevelCache.Hits);

            //Item should be in instance 1 first level cache
            //No hits on second level cache
            Assert.Equal("ver", instance1.Get<string>(key));
            Assert.Equal(2, secondLevelCache.Hits);

            //Item should be fetched into instance 2 first level
            //cache
            //2 hits on second level cache
            Assert.Equal("ver", instance2.Get<string>(key));
            Assert.Equal(4, secondLevelCache.Hits);

            //Update item from instance 1
            //2 hits on second level cache
            instance1.Set(key, "ver2");
            Assert.Equal(6, secondLevelCache.Hits);

            //Instance 1 should have the correct item, because it set 
            //the new item
            //No hits on second level cache
            Assert.Equal("ver2", instance1.Get<string>(key));
            Assert.Equal(6, secondLevelCache.Hits);

            //Instance 2 should not have the correct item yet, because
            //the firstlevel cache has not been cleared. In a prod
            //scenario the first level cache will check the cache version
            //in the next request
            //No hits on second level cache
            Assert.Equal("ver", instance2.Get<string>(key));
            Assert.Equal(6, secondLevelCache.Hits);

            //But if we clear instance 2's first level cache it should
            //get the new item
            //2 hits on second level cache
            instance2.InvalidateFirstLevelCache();
            Assert.Equal("ver2", instance2.Get<string>(key));
            Assert.Equal(8, secondLevelCache.Hits);

            //Instance 1 removes the item
            //2 hits on second level cache
            instance1.Remove(key);
            Assert.Equal(10, secondLevelCache.Hits);

            //Instance 1 first level cache should have been updated
            //No hits on second level cache
            Assert.Null(instance1.Get<string>(key));
            Assert.Equal(10, secondLevelCache.Hits);

            //Instance 2 first level cache should not have 
            //been updated. In a prod scenario the first level
            //cache will check the cache version in the next request
            //No hits on second level cache
            Assert.NotNull(instance2.Get<string>(key));

            //If we clear instance 2 first level cache the
            //item should have been removed
            //2 hits on second level cache
            instance2.InvalidateFirstLevelCache();
            Assert.Null(instance2.Get<string>(key));
            Assert.Equal(12, secondLevelCache.Hits);
        }

        #region Mocks
        private class DistributedCacheMock : IDistributedCache
        {

            public int Gets { get; set; } = 0;
            public int Sets { get; set; } = 0;
            public int Removes { get; set; } = 0;
            public int Hits => Gets + Sets + Removes;
            public readonly Dictionary<string, byte[]> Cache = new();

            public byte[] Get(string key)
            {
                Gets++;
                if (Cache.TryGetValue(key,  out byte[] data))
                {
                    return data;
                }
                return null;
            }

            public Task<byte[]> GetAsync(string key, CancellationToken token = new CancellationToken())
            {
                throw new NotImplementedException();
            }

            public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
            {
                Sets++;
                Cache[key] = value;
            }

            public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options,
                CancellationToken token = new CancellationToken())
            {
                throw new NotImplementedException();
            }

            public void Refresh(string key)
            {
                throw new NotImplementedException();
            }

            public Task RefreshAsync(string key, CancellationToken token = new CancellationToken())
            {
                throw new NotImplementedException();
            }

            public void Remove(string key)
            {
                Removes++;
                if (Cache.ContainsKey(key))
                {
                    Cache.Remove(key);
                }
            }

            public Task RemoveAsync(string key, CancellationToken token = new CancellationToken())
            {
                throw new NotImplementedException();
            }
        }

        private class MemoryCacheMock : IMemoryCache
        {
            public IList<ICacheEntry> CacheEntries = new List<ICacheEntry>();
            public int Gets { get; set; } = 0;
            public int Sets { get; set; } = 0;
            public int Removes { get; set; } = 0;
            public int Total => Gets + Sets + Removes;

            public MemoryCacheMock()
            {
            }

            public void Dispose()
            {
            }

            public bool TryGetValue(object key, out object value)
            {
                Gets++;
                var tmp = CacheEntries.SingleOrDefault(o => o.Key.ToString() == key.ToString());
                if (tmp != null)
                {
                    value = tmp.Value;
                    return true;
                }

                value = null;
                return false;
            }

            public ICacheEntry CreateEntry(object key)
            {
                Sets++;
                var tmp = CacheEntries.SingleOrDefault(o => o.Key.ToString() == key.ToString());

                if (tmp == null)
                {
                    tmp = new CacheEntryMock()
                    {
                        Key = key
                    };
                    CacheEntries.Add(tmp);
                }
                
                return tmp;
            }

            public void Remove(object key)
            {
                Removes++;
                CacheEntries = CacheEntries.Where(o => o.Key.ToString() != key.ToString()).ToList();
            }

        }

        private class CacheEntryMock : ICacheEntry
        {
            public void Dispose()
            {
            }

            public object Key { get; set; }
            public object Value { get; set; }
            public DateTimeOffset? AbsoluteExpiration { get; set; }
            public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
            public TimeSpan? SlidingExpiration { get; set; }
            public IList<IChangeToken> ExpirationTokens { get; }
            public IList<PostEvictionCallbackRegistration> PostEvictionCallbacks { get; }
            public CacheItemPriority Priority { get; set; }
            public long? Size { get; set; }
        }

        #endregion

    }
}