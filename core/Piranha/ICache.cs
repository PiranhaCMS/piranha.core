/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

namespace Piranha
{
    public interface ICache
    {
        /// <summary>
        /// Gets the model with the specified key from cache.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="key">The unique key</param>
        /// <returns>The cached model, null it wasn't found</returns>
        T Get<T>(string key);

        /// <summary>
        /// Sets the given model in the cache.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="key">The unique key</param>
        /// <param name="value">The model</param>
        void Set<T>(string key, T value);

        /// <summary>
        /// Removes the model with the specified key from cache.
        /// </summary>
        /// <param name="key">The unique key</param>
        void Remove(string key);
    }
}
