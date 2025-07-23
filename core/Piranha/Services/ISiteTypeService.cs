﻿/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Models;

namespace Piranha.Services;

public interface ISiteTypeService
{
    /// <summary>
    /// Gets all available models.
    /// </summary>
    /// <returns>The available models</returns>
    Task<IEnumerable<SiteType>> GetAllAsync();

    /// <summary>
    /// Gets the model with the specified id.
    /// </summary>
    /// <param name="id">The unique i</param>
    /// <returns></returns>
    Task<SiteType> GetByIdAsync(string id);

    /// <summary>
    /// Adds or updates the given model in the database
    /// depending on its state.
    /// </summary>
    /// <param name="model">The model</param>
    Task SaveAsync(SiteType model);

    /// <summary>
    /// Deletes the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    Task DeleteAsync(string id);

    /// <summary>
    /// Deletes the given model.
    /// </summary>
    /// <param name="model">The model</param>
    Task DeleteAsync(SiteType model);

    /// <summary>
    /// Deletes the given models.
    /// </summary>
    /// <param name="models">The models</param>
    Task DeleteAsync(IEnumerable<SiteType> models);
}
