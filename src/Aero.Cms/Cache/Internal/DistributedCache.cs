

using Microsoft.Extensions.Caching.Distributed;
using Aero.Cms.Models;


namespace Aero.Cms.Cache;

/// <inheritdoc />
internal sealed class DistributedCache : ICache
{
    private readonly IDistributedCache _cache;
    private readonly JsonSerializerOptions _jsonSettings;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="cache">The currently configured cache</param>
    public DistributedCache(IDistributedCache cache)
    {
        _cache = cache;

        var typeInfoResolver = new System.Text.Json.Serialization.Metadata.DefaultJsonTypeInfoResolver();
        typeInfoResolver.Modifiers.Add(info =>
        {
            if (typeof(PageBase).IsAssignableFrom(info.Type))
            {
                info.PolymorphismOptions = null;
            }
        });

        _jsonSettings = new JsonSerializerOptions()
        {
            IncludeFields = true,
            PropertyNameCaseInsensitive = true,
            TypeInfoResolver = typeInfoResolver
        };
    }

    /// <inheritdoc />
    public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var json = await _cache.GetStringAsync(key, cancellationToken).ConfigureAwait(false);

        if (!string.IsNullOrEmpty(json))
        {
            return (T)JsonSerializer.Deserialize(json, typeof(T), _jsonSettings);
        }
        return default;
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        await _cache.SetStringAsync(key, JsonSerializer.Serialize((object)value, _jsonSettings), cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key, cancellationToken).ConfigureAwait(false);
    }
}
