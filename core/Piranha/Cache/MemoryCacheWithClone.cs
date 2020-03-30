/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using Microsoft.Extensions.Caching.Memory;

namespace Piranha.Cache
{
    /// <summary>
    /// Simple in memory cache.
    /// </summary>
    public class MemoryCacheWithClone : MemoryCache
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="cache">The currently configured cache</param>
        public MemoryCacheWithClone(IMemoryCache cache) : base(cache, true) { }
    }
}
