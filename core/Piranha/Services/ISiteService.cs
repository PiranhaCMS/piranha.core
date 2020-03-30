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

namespace Piranha.Services
{
    public interface ISiteService
    {
        /// <summary>
        /// Gets all available models.
        /// </summary>
        /// <returns>The available models</returns>
        Task<IEnumerable<Site>> GetAllAsync();

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The model, or null if it doesn't exist</returns>
        Task<Site> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets the model with the given internal id.
        /// </summary>
        /// <param name="internalId">The unique internal i</param>
        /// <returns>The model</returns>
        Task<Site> GetByInternalIdAsync(string internalId);

        /// <summary>
        /// Gets the model with the given hostname.
        /// </summary>
        /// <param name="hostname">The hostname</param>
        /// <returns>The model</returns>
        Task<Site> GetByHostnameAsync(string hostname);

        /// <summary>
        /// Gets the default side.
        /// </summary>
        /// <returns>The modell, or NULL if it doesnt exist</returns>
        Task<Site> GetDefaultAsync();

        /// <summary>
        /// Gets the site content for given site id.
        /// </summary>
        /// <param name="id">Site id</param>
        /// <returns>The site content model</returns>
        Task<DynamicSiteContent> GetContentByIdAsync(Guid id);

        /// <summary>
        /// Gets the site content for given site id.
        /// </summary>
        /// <param name="id">Site id</param>
        /// <typeparam name="T">The site model type</typeparam>
        /// <returns>The site content model</returns>
        Task<T> GetContentByIdAsync<T>(Guid id) where T : SiteContent<T>;

        /// <summary>
        /// Gets the hierachical sitemap structure.
        /// </summary>
        /// <param name="id">The optional site id</param>
        /// <param name="onlyPublished">If only published items should be included</param>
        /// <returns>The sitemap</returns>
        Task<Sitemap> GetSitemapAsync(Guid? id = null, bool onlyPublished = true);

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        Task SaveAsync(Site model);

        /// <summary>
        /// Saves the given site content to the site with the
        /// given id.
        /// </summary>
        /// <param name="siteId">The site id</param>
        /// <param name="model">The site content model</param>
        /// <typeparam name="T">The site content type</typeparam>
        Task SaveContentAsync<T>(Guid siteId, T model) where T : SiteContent<T>;

        /// <summary>
        /// Creates and initializes a new site content model of the specified type.
        /// </summary>
        /// <returns>The created site content</returns>
        Task<T> CreateContentAsync<T>(string typeId = null) where T : SiteContentBase;

        /// <summary>
        /// Invalidates the cached version of the sitemap with the
        /// given id, if caching is enabled.
        /// </summary>
        /// <param name="id">The site id</param>
        /// <param name="updateLastModified">If the global last modified date should be updated</param>
        Task InvalidateSitemapAsync(Guid id, bool updateLastModified = true);

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        Task DeleteAsync(Site model);

        /// <summary>
        /// Removes the sitemap from the cache.
        /// </summary>
        /// <param name="id">The unique id</param>
        Task RemoveSitemapFromCacheAsync(Guid id);
    }
}
