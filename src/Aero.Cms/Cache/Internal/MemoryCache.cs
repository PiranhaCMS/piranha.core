

using Microsoft.Extensions.Caching.Memory;

namespace Aero.Cms.Cache;

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
    public Task<T> GetAsync<T>(string key, CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue<T>(key, out var obj))
        {
            return Task.FromResult(obj);
        }
        return Task.FromResult(default(T));
    }

    /// <inheritdoc />
    public Task SetAsync<T>(string key, T value, CancellationToken cancellationToken)
    {
        _cache.Set(key, value);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RemoveAsync(string key, CancellationToken cancellationToken)
    {
        _cache.Remove(key);
        return Task.CompletedTask;
    }
}
