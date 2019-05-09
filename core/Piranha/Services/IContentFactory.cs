/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using Piranha.Models;

namespace Piranha.Services
{
    public interface IContentFactory
    {
        /// <summary>
        /// Creates and initializes a new content model.
        /// </summary>
        /// <param name="type">The content type</param>
        /// <typeparam name="T">The model type</typeparam>
        /// <returns>The new model</returns>
        T Create<T>(ContentType type) where T : Content;

        /// <summary>
        /// Creates a new dynamic region.
        /// </summary>
        /// <param name="type">The content type</param>
        /// <param name="regionId">The region id</param>
        /// <returns>The new region value</returns>
        object CreateDynamicRegion(ContentType type, string regionId);

        /// <summary>
        /// Creates and initializes a new block of the specified type.
        /// </summary>
        /// <param name="typeName">The type name</param>
        /// <returns>The new block</returns>
        object CreateBlock(string typeName);

        /// <summary>
        /// Initializes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="type">The content type</param>
        /// <typeparam name="T">The model type</typeparam>
        /// <returns>The initialized model</returns>
        T Init<T>(T model, ContentType type) where T : Content;

        /// <summary>
        /// Initializes the given dynamic model.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="type">The content type</param>
        /// <typeparam name="T">The model type</typeparam>
        /// <returns>The initialized model</returns>
        T InitDynamic<T>(T model, ContentType type) where T : IDynamicModel;
    }
}