/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Piranha.Cache;

/// <inheritdoc />
internal sealed class DistributedCache : ICache
{
    private readonly IDistributedCache _cache;
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

    /// <inheritdoc />
    public T Get<T>(string key)
    {
        var json = _cache.GetString(key);

        if (!string.IsNullOrEmpty(json))
        {
            return JsonConvert.DeserializeObject<T>(json, _jsonSettings);
        }
        return default(T);
    }

    /// <inheritdoc />
    public void Set<T>(string key, T value)
    {
        _cache.SetString(key, JsonConvert.SerializeObject(value, _jsonSettings));
    }

    /// <inheritdoc />
    public void Remove(string key)
    {
        _cache.Remove(key);
    }
}
