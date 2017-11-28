/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Collections.Generic;

namespace Piranha.Repositories
{
    public interface IPageRepository
    {
        /// <summary>
        /// Gets all of the available pages for the current site.
        /// </summary>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The pages</returns>
        IEnumerable<Models.DynamicPage> GetAll(string siteId = null);

        /// <summary>
        /// Gets the site startpage.
        /// </summary>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The page model</returns>
        Models.DynamicPage GetStartpage(string siteId = null);

        /// <summary>
        /// Gets the site startpage.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The page model</returns>
        T GetStartpage<T>(string siteId = null) where T : Models.Page<T>;

        /// <summary>
        /// Gets the page model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The page model</returns>
        Models.DynamicPage GetById(string id);

        /// <summary>
        /// Gets the page model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <returns>The page model</returns>
        T GetById<T>(string id) where T : Models.Page<T>;

        /// <summary>
        /// Gets the page model with the specified slug.
        /// </summary>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The page model</returns>
        Models.DynamicPage GetBySlug(string slug, string siteId = null);

        /// <summary>
        /// Gets the page model with the specified slug.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The page model</returns>
        T GetBySlug<T>(string slug, string siteId = null) where T : Models.Page<T>;

        /// <summary>
        /// Moves the current page in the structure.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="model">The page to move</param>
        /// <param name="parentId">The new parent id</param>
        /// <param name="sortOrder">The new sort order</param>
        void Move<T>(T model, string parentId, int sortOrder) where T : Models.Page<T>;

        /// <summary>
        /// Saves the given page model
        /// </summary>
        /// <param name="model">The page model</param>
        void Save<T>(T model) where T : Models.Page<T>;

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        void Delete(string id);

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        void Delete<T>(T model) where T : Models.Page<T>;
    }
}
