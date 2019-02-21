/*
 * Copyright (c) 2016-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Piranha.Repositories
{
    public interface IPageRepository
    {
        /// <summary>
        /// Creates and initializes a new page of the specified type.
        /// </summary>
        /// <returns>The created page</returns>
        Task<T> Create<T>(string typeId = null) where T : Models.PageBase;

        /// <summary>
        /// Creates and initializes a copy of the given page.
        /// </summary>
        /// <returns>The created copy</returns>
        Task<T> Copy<T>(T originalPage) where T : Models.PageBase;

        /// <summary>
        /// Gets all of the available pages for the current site.
        /// </summary>
        /// <param name="siteId">The site id</param>
        /// <returns>The pages</returns>
        Task<IEnumerable<Guid>> GetAll(Guid siteId);

        /// <summary>
        /// Gets the available blog pages for the current site.
        /// </summary>
        /// <param name="siteId">The site id</param>
        /// <returns>The pages</returns>
        Task<IEnumerable<Guid>> GetAllBlogs(Guid siteId);

        /// <summary>
        /// Gets the site startpage.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="siteId">The site id</param>
        /// <returns>The page model</returns>
        Task<T> GetStartpage<T>(Guid siteId) where T : Models.PageBase;

        /// <summary>
        /// Gets the page model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <returns>The page model</returns>
        Task<T> GetById<T>(Guid id) where T : Models.PageBase;

        /// <summary>
        /// Gets the page model with the specified slug.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The site id</param>
        /// <returns>The page model</returns>
        Task<T> GetBySlug<T>(string slug, Guid siteId) where T : Models.PageBase;

        /// <summary>
        /// Moves the current page in the structure.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="model">The page to move</param>
        /// <param name="parentId">The new parent id</param>
        /// <param name="sortOrder">The new sort order</param>
        /// <returns>The other pages that were affected by the move</returns>
        Task<IEnumerable<Guid>> Move<T>(T model, Guid? parentId, int sortOrder) where T : Models.PageBase;

        /// <summary>
        /// Saves the given page model
        /// </summary>
        /// <param name="model">The page model</param>
        /// <returns>The other pages that were affected by the move</returns>
        Task<IEnumerable<Guid>> Save<T>(T model) where T : Models.PageBase;

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The other pages that were affected by the move</returns>
        Task<IEnumerable<Guid>> Delete(Guid id);
    }
}
