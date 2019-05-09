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
using Piranha.Models;

namespace Piranha.Repositories
{
    public interface IPostRepository
    {
        /// <summary>
        /// Gets the available posts for the specified blog.
        /// </summary>
        /// <param name="blogId">The unique id</param>
        /// <returns>The posts</returns>
        Task<IEnumerable<Guid>> GetAll(Guid blogId);

        /// <summary>
        /// Gets the available posts for the specified site.
        /// </summary>
        /// <param name="siteId">The site id</param>
        /// <returns>The posts</returns>
        Task<IEnumerable<Guid>> GetAllBySiteId(Guid siteId);

        /// <summary>
        /// Gets all available categories for the specified blog.
        /// </summary>
        /// <param name="id">The blog id</param>
        /// <returns>The available categories</returns>
        Task<IEnumerable<Taxonomy>> GetAllCategories(Guid blogId);

        /// <summary>
        /// Gets all available tags for the specified blog.
        /// </summary>
        /// <param name="id">The blog id</param>
        /// <returns>The available tags</returns>
        Task<IEnumerable<Taxonomy>> GetAllTags(Guid blogId);

        /// <summary>
        /// Gets the post model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <returns>The post model</returns>
        Task<T> GetById<T>(Guid id) where T : PostBase;

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="blogId">The blog id</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The post model</returns>
        Task<T> GetBySlug<T>(Guid blogId, string slug) where T : PostBase;

        /// <summary>
        /// Gets the category with the given slug.
        /// </summary>
        /// <param name="blogId">The blog id</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The category</returns>
        Task<Taxonomy> GetCategoryBySlug(Guid blogId, string slug);

        /// <summary>
        /// Gets the tag with the given slug.
        /// </summary>
        /// <param name="blogId">The blog id</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The tag</returns>
        Task<Taxonomy> GetTagBySlug(Guid blogId, string slug);

        /// <summary>
        /// Saves the given post model
        /// </summary>
        /// <param name="model">The post model</param>
        Task Save<T>(T model) where T : PostBase;

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        Task Delete(Guid id);
    }
}
