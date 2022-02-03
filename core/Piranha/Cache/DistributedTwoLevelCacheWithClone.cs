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
    public class DistributedTwoLevelCacheWithClone : DistributedTwoLevelCache
    {
        protected DistributedTwoLevelCacheWithClone(IMemoryCache firstLevelCache, IDistributedCache secondLevelCache) : base(firstLevelCache, secondLevelCache, true)
        {
        }
    }


}
