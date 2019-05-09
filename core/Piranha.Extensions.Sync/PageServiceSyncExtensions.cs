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
    public static class PageServiceSyncExtensions
    {
        /// <summary>
        /// Detaches a copy and initializes it as a standalone page
        /// </summary>
        /// <returns>The standalone page</returns>
        public static void Detach<T>(this IPageService service, T model) where T : PageBase
        {
            service.DetachAsync<T>(model).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets all available models.
        /// </summary>
        /// <returns>The available models</returns>
        public static IEnumerable<DynamicPage> GetAll(this IPageService service, Guid? siteId = null)
        {
            return service.GetAllAsync(siteId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets all available models.
        /// </summary>
        /// <returns>The available models</returns>
        public static IEnumerable<T> GetAll<T>(this IPageService service, Guid? siteId = null) where T : PageBase
        {
            return service.GetAllAsync<T>(siteId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the available blog pages for the current site.
        /// </summary>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The pages</returns>
        public static IEnumerable<DynamicPage> GetAllBlogs(this IPageService service, Guid? siteId = null)
        {
            return service.GetAllBlogsAsync(siteId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the available blog pages for the current site.
        /// </summary>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The pages</returns>
        public static IEnumerable<T> GetAllBlogs<T>(this IPageService service, Guid? siteId = null) where T : PageBase
        {
            return service.GetAllBlogsAsync<T>(siteId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the site startpage.
        /// </summary>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The page model</returns>
        public static DynamicPage GetStartpage(this IPageService service, Guid? siteId = null)
        {
            return service.GetStartpageAsync(siteId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the site startpage.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The page model</returns>
        public static T GetStartpage<T>(this IPageService service, Guid? siteId = null) where T : PageBase
        {
            return service.GetStartpageAsync<T>(siteId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the page model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The page model</returns>
        public static DynamicPage GetById(this IPageService service, Guid id)
        {
            return service.GetByIdAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The model, or null if it doesn't exist</returns>
        public static T GetById<T>(this IPageService service, Guid id) where T : PageBase
        {
            return service.GetByIdAsync<T>(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the page model with the specified slug.
        /// </summary>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The page model</returns>
        public static DynamicPage GetBySlug(this IPageService service, string slug, Guid? siteId = null)
        {
            return service.GetBySlugAsync(slug, siteId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the page model with the specified slug.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The page model</returns>
        public static T GetBySlug<T>(this IPageService service, string slug, Guid? siteId = null) where T : PageBase
        {
            return service.GetBySlugAsync<T>(slug, siteId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the id for the page with the given slug.
        /// </summary>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The optional page id</param>
        /// <returns>The id</returns>
        public static Guid? GetIdBySlug(this IPageService service, string slug, Guid? siteId = null)
        {
            return service.GetIdBySlugAsync(slug, siteId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Moves the current page in the structure.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="model">The page to move</param>
        /// <param name="parentId">The new parent id</param>
        /// <param name="sortOrder">The new sort order</param>
        public static void Move<T>(this IPageService service, T model, Guid? parentId, int sortOrder) where T : PageBase
        {
            service.MoveAsync(model, parentId, sortOrder).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Saves the given page model
        /// </summary>
        /// <param name="model">The page model</param>
        public static void Save<T>(this IPageService service, T model) where T : PageBase
        {
            service.SaveAsync(model).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public static void Delete(this IPageService service, Guid id)
        {
            service.DeleteAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        public static void Delete<T>(this IPageService service, T model) where T : PageBase
        {
            service.DeleteAsync(model).GetAwaiter().GetResult();
        }
    }
}
