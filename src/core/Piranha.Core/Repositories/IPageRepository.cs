/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using System.Collections.Generic;
using Piranha.Models;

namespace Piranha.Repositories
{
    /// <summary>
    /// The client page repository interface.
    /// </summary>
    public interface IPageRepository
    {
        /// <summary>
        /// Gets the site startpage.
        /// </summary>
        /// <returns>The page model</returns>
        PageModel GetStartpage();

        /// <summary>
        /// Gets the site startpage.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <returns>The page model</returns>
        T GetStartpage<T>() where T : PageModel<T>;

        /// <summary>
        /// Gets the page model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The page model</returns>
        PageModel GetById(Guid id);

        /// <summary>
        /// Gets the page model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <returns>The page model</returns>
        T GetById<T>(Guid id) where T : PageModel<T>;

        /// <summary>
        /// Gets the page model with the specified slug.
        /// </summary>
        /// <param name="slug">The unique slug</param>
        /// <returns>The page model</returns>
        PageModel GetBySlug(string slug);

        /// <summary>
        /// Gets the page model with the specified slug.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="slug">The unique slug</param>
        /// <returns>The page model</returns>
        T GetBySlug<T>(string slug) where T : PageModel<T>;

        /// <summary>
        /// Gets all page models with the specified parent id.
        /// </summary>
        /// <param name="parentId">The parent id</param>
        /// <returns>The page models</returns>
        IList<PageModel> GetByParentId(Guid? parentId);

        /// <summary>
        /// Saves the given page model
        /// </summary>
        /// <param name="model">The page model</param>
        void Save<T>(T model) where T : PageModel<T>;

        /// <summary>
        /// Deletes the given page.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="model">The page to delete</param>
        void Delete<T>(T model) where T : PageModel<T>;

        /// <summary>
        /// Moves the current page in the structure.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="model">The page to move</param>
        /// <param name="parentId">The new parent id</param>
        /// <param name="sortOrder">The new sort order</param>
        void Move<T>(T model, Guid? parentId, int sortOrder) where T : PageModel<T>;

        /// <summary>
        /// Delets the page with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        void Delete(Guid id);
    }
}
