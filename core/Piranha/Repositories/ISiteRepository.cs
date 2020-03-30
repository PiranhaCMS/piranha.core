/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Piranha.Models;

namespace Piranha.Repositories
{
    public interface ISiteRepository
    {
        /// <summary>
        /// Gets all available models.
        /// </summary>
        /// <returns>The available models</returns>
        Task<IEnumerable<Site>> GetAll();

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The model, or NULL if it doesn't exist</returns>
        Task<Site> GetById(Guid id);

        /// <summary>
        /// Gets the model with the given internal id.
        /// </summary>
        /// <param name="internalId">The unique internal i</param>
        /// <returns>The model</returns>
        Task<Site> GetByInternalId(string internalId);

        /// <summary>
        /// Gets the default side.
        /// </summary>
        /// <returns>The modell, or NULL if it doesnt exist</returns>
        Task<Site> GetDefault();

        /// <summary>
        /// Gets the site content for given site id.
        /// </summary>
        /// <param name="id">Site id</param>
        /// <returns>The site content model</returns>
        Task<DynamicSiteContent> GetContentById(Guid id);

        /// <summary>
        /// Gets the site content for given site id.
        /// </summary>
        /// <param name="id">Site id</param>
        /// <typeparam name="T">The site model type</typeparam>
        /// <returns>The site content model</returns>
        Task<T> GetContentById<T>(Guid id) where T : SiteContent<T>;

        /// <summary>
        /// Gets the hierachical sitemap structure.
        /// </summary>
        /// <param name="id">The site id</param>
        /// <param name="onlyPublished">If only published items should be included</param>
        /// <returns>The sitemap</returns>
        Task<Sitemap> GetSitemap(Guid id, bool onlyPublished = true);

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        Task Save(Site model);

        /// <summary>
        /// Saves the given site content to the site with the
        /// given id.
        /// </summary>
        /// <param name="siteId">The site id</param>
        /// <param name="content">The site content</param>
        /// <typeparam name="T">The site content type</typeparam>
        Task SaveContent<T>(Guid siteId, T content) where T : SiteContent<T>;

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        Task Delete(Guid id);
    }
}
