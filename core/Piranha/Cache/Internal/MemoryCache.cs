/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.Extensions.Caching.Memory;

namespace Piranha.Cache;

/// <inheritdoc />
internal sealed class MemoryCache : ICache
{
    private readonly IMemoryCache _cache;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="cache">The currently configured cache</param>
    public MemoryCache(IMemoryCache cache)
    {
        _cache = cache;
    }

    /// <inheritdoc />
    public T Get<T>(string key)
    {
        if (_cache.TryGetValue<T>(key, out var obj))
        {
            return obj;
        }
        return default(T);
    }

    /// <inheritdoc />
    public void Set<T>(string key, T value)
    {
        _cache.Set(key, value);
    }

    /// <inheritdoc />
    public void Remove(string key)
    {
        _cache.Remove(key);
    }
}
