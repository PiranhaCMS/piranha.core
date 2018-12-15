/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System;
using System.Collections.Generic;
using Piranha.Data;

namespace Piranha.Repositories
{
    public interface ISiteRepository
    {
        /// <summary>
        /// Gets all available models.
        /// </summary>
        /// <returns>The available models</returns>
        IEnumerable<Site> GetAll();

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The model, or NULL if it doesn't exist</returns>
        Site GetById(Guid id);

        /// <summary>
        /// Gets the model with the given internal id.
        /// </summary>
        /// <param name="internalId">The unique internal i</param>
        /// <returns>The model</returns>
        Site GetByInternalId(string internalId);

        /// <summary>
        /// Gets the model with the given hostname.
        /// </summary>
        /// <param name="hostname">The hostname</param>
        /// <returns>The model</returns>
        Site GetByHostname(string hostname);

        /// <summary>
        /// Gets the default side.
        /// </summary>
        /// <returns>The modell, or NULL if it doesnt exist</returns>
        Site GetDefault();

        /// <summary>
        /// Gets the site content for given site id.
        /// </summary>
        /// <param name="id">Site id</param>
        /// <returns>The site content model</returns>
        Models.DynamicSiteContent GetContentById(Guid id);

        /// <summary>
        /// Gets the site content for given site id.
        /// </summary>
        /// <param name="id">Site id</param>
        /// <typeparam name="T">The site model type</typeparam>
        /// <returns>The site content model</returns>
        T GetContentById<T>(Guid id) where T : Models.SiteContent<T>;

        /// <summary>
        /// Gets the hierachical sitemap structure.
        /// </summary>
        /// <param name="id">The optional site id</param>
        /// <param name="onlyPublished">If only published items should be included</param>
        /// <returns>The sitemap</returns>
        Models.Sitemap GetSitemap(Guid? id = null, bool onlyPublished = true);

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        void Save(Site model);

        /// <summary>
        /// Saves the given site content to the site with the 
        /// given id.
        /// </summary>
        /// <param name="siteId">The site id</param>
        /// <param name="content">The site content</param>
        /// <typeparam name="T">The site content type</typeparam>
        void SaveContent<T>(Guid siteId, T content) where T : Models.SiteContent<T>;

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        void Delete(Guid id);

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        void Delete(Site model);

        /// <summary>
        /// Creates and initializes a new site content model of the specified type.
        /// </summary>
        /// <returns>The created site content</returns>
        T CreateContent<T>(string typeId = null) where T : Models.SiteContentBase;

        /// <summary>
        /// Invalidates the cached version of the sitemap with the
        /// given id, if caching is enabled.
        /// </summary>
        /// <param name="id">The site id</param>
        void InvalidateSitemap(Guid id);
    }
}
