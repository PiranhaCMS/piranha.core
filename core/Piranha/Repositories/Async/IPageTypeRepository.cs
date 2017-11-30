/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Piranha.Repositories.Async
{
    public interface IPageTypeRepository
    {
        /// <summary>
        /// Gets all available models.
        /// </summary>
        /// <param name="transaction">The optional transaction</param>
        /// <returns>The available models</returns>
        Task<IEnumerable<Models.PageType>> GetAll(IDbTransaction transaction = null);

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="id">The unique i</param>
        /// <param name="transaction">The optional transaction</param>
        /// <returns></returns>
        Task<Models.PageType> GetById(string id, IDbTransaction transaction = null);

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="transaction">The optional transaction</param>
        Task Save(Models.PageType model, IDbTransaction transaction = null);

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="transaction">The optional transaction</param>
        Task Delete(string id, IDbTransaction transaction = null);

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="transaction">The optional transaction</param>
        Task Delete(Models.PageType model, IDbTransaction transaction = null);
    }
}
