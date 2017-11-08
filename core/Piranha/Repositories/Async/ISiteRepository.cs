﻿/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Piranha.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Piranha.Repositories.Async
{
    public interface ISiteRepository
    {
        /// <summary>
        /// Gets all available models.
        /// </summary>
        /// <param name="transaction">The optional transaction</param>
        /// <returns>The available models</returns>
        Task<IEnumerable<Site>> GetAll(IDbTransaction transaction = null);

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="transaction">The optional transaction</param>
        /// <returns>The model, or NULL if it doesn't exist</returns>
        Task<Site> GetById(string id, IDbTransaction transaction = null);

        /// <summary>
        /// Gets the model with the given internal id.
        /// </summary>
        /// <param name="internalId">The unique internal i</param>
        /// <param name="transaction">The optional transaction</param>
        /// <returns>The model</returns>
        Task<Site> GetByInternalId(string internalId, IDbTransaction transaction = null);

        /// <summary>
        /// Gets the default side.
        /// </summary>
        /// <param name="transaction">The optional transaction</param>
        /// <returns>The modell, or NULL if it doesnt exist</returns>
        Task<Site> GetDefault(IDbTransaction transaction = null);

        /// <summary>
        /// Gets the hierachical Task<Site>map structure.
        /// </summary>
        /// <param name="id">The optional Task<Site> id</param>
        /// <param name="onlyPublished">If only published items should be included</param>
        /// <param name="transaction">The optional transaction</param>
        /// <returns>The Task<Site>map</returns>
        Task<Models.Sitemap> GetSitemap(string id = null, bool onlyPublished = true, IDbTransaction transaction = null);

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="transaction">The optional transaction</param>
        Task Save(Site  model, IDbTransaction transaction = null);

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
        Task Delete(Site model, IDbTransaction transaction = null);
    }
}
