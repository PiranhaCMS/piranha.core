/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using Piranha.Models;

namespace Piranha.Services
{
    public interface IPostTypeService
    {
        /// <summary>
        /// Gets all available models.
        /// </summary>
        /// <returns>The available models</returns>
        Task<IEnumerable<PostType>> GetAllAsync();

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="id">The unique i</param>
        /// <returns></returns>
        Task<PostType> GetByIdAsync(string id);

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        Task SaveAsync(PostType model);

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        Task DeleteAsync(string id);

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        Task DeleteAsync(PostType model);
    }
}
