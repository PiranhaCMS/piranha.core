/*
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

public interface ILanguageService
{
    /// <summary>
    /// Gets all available models.
    /// </summary>
    /// <returns>The available models</returns>
    Task<IEnumerable<Language>> GetAllAsync();

    /// <summary>
    /// Gets the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <returns>The model, or null if it doesn't exist</returns>
    Task<Language> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets the default side.
    /// </summary>
    /// <returns>The modell</returns>
    Task<Language> GetDefaultAsync();

    /// <summary>
    /// Adds or updates the given model in the database
    /// depending on its state.
    /// </summary>
    /// <param name="model">The model</param>
    Task SaveAsync(Language model);

    /// <summary>
    /// Deletes the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Deletes the given model.
    /// </summary>
    /// <param name="model">The model</param>
    Task DeleteAsync(Language model);
}
