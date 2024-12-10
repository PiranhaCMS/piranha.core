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
    public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var json = await _cache.GetStringAsync(key, cancellationToken).ConfigureAwait(false);

        if (!string.IsNullOrEmpty(json))
        {
            return JsonConvert.DeserializeObject<T>(json, _jsonSettings);
        }
        return default;
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        await _cache.SetStringAsync(key, JsonConvert.SerializeObject(value, _jsonSettings), cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key, cancellationToken).ConfigureAwait(false);
    }
}
