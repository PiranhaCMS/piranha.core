/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Cache;

/// <summary>
/// Service for handling model caching.
/// </summary>
public interface ICache
{
    /// <summary>
    /// Gets the model with the specified key from cache.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    /// <param name="key">The unique key</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The cached model, null it wasn't found</returns>
    Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the given model in the cache.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    /// <param name="key">The unique key</param>
    /// <param name="value">The model</param>
    /// <param name="cancellationToken"></param>
    Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the model with the specified key from cache.
    /// </summary>
    /// <param name="key">The unique key</param>
    /// <param name="cancellationToken"></param>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}
