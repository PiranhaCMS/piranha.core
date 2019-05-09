/*
 * Copyright (c) 2019 Håkan Edling
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
    public static class SiteServiceSyncExtensions
    {
        /// <summary>
        /// Gets all available models.
        /// </summary>
        /// <returns>The available models</returns>
        public static IEnumerable<Site> GetAll(this ISiteService service)
        {
            return service.GetAllAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The model, or null if it doesn't exist</returns>
        public static Site GetById(this ISiteService service, Guid id)
        {
            return service.GetByIdAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the model with the given internal id.
        /// </summary>
        /// <param name="internalId">The unique internal i</param>
        /// <returns>The model</returns>
        public static Site GetByInternalId(this ISiteService service, string internalId)
        {
            return service.GetByInternalIdAsync(internalId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the model with the given hostname.
        /// </summary>
        /// <param name="hostname">The hostname</param>
        /// <returns>The model</returns>
        public static Site GetByHostname(this ISiteService service, string hostname)
        {
            return service.GetByHostnameAsync(hostname).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the default side.
        /// </summary>
        /// <returns>The modell, or NULL if it doesnt exist</returns>
        public static Site GetDefault(this ISiteService service)
        {
            return service.GetDefaultAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the site content for given site id.
        /// </summary>
        /// <param name="id">Site id</param>
        /// <returns>The site content model</returns>
        public static DynamicSiteContent GetContentById(this ISiteService service, Guid id)
        {
            return service.GetContentByIdAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the site content for given site id.
        /// </summary>
        /// <param name="id">Site id</param>
        /// <typeparam name="T">The site model type</typeparam>
        /// <returns>The site content model</returns>
        public static T GetContentById<T>(this ISiteService service, Guid id) where T : SiteContent<T>
        {
            return service.GetContentByIdAsync<T>(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the hierachical sitemap structure.
        /// </summary>
        /// <param name="id">The optional site id</param>
        /// <param name="onlyPublished">If only published items should be included</param>
        /// <returns>The sitemap</returns>
        public static Sitemap GetSitemap(this ISiteService service, Guid? id = null, bool onlyPublished = true)
        {
            return service.GetSitemapAsync(id, onlyPublished).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        public static void Save(this ISiteService service, Site model)
        {
            service.SaveAsync(model).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Saves the given site content to the site with the
        /// given id.
        /// </summary>
        /// <param name="siteId">The site id</param>
        /// <param name="model">The site content model</param>
        /// <typeparam name="T">The site content type</typeparam>
        public static void SaveContent<T>(this ISiteService service, Guid siteId, T model) where T : SiteContent<T>
        {
            service.SaveContentAsync(siteId, model).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public static void Delete(this ISiteService service, Guid id)
        {
            service.DeleteAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        public static void Delete(this ISiteService service, Site model)
        {
            service.DeleteAsync(model).GetAwaiter().GetResult();
        }
    }
}
