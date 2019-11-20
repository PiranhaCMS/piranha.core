/*
 * Copyright (c) 2019 Håkan Edling
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

namespace Piranha.Services
{
    public interface IPostService
    {
        /// <summary>
        /// Creates and initializes a new post of the specified type.
        /// </summary>
        /// <returns>The created post</returns>
        T Create<T>(string typeId = null) where T : PostBase;

        /// <summary>
        /// Gets the available posts for the specified blog.
        /// </summary>
        /// <param name="blogId">The unique blog id</param>
        /// <returns>The posts</returns>
        Task<IEnumerable<DynamicPost>> GetAllAsync(Guid blogId, int? index = null, int? pageSize = null);

        /// <summary>
        /// Gets the available post items.
        /// </summary>
        /// <param name="blogId">The unique id</param>
        /// <returns>The posts</returns>
        Task<IEnumerable<T>> GetAllAsync<T>(Guid blogId, int? index = null, int? pageSize = null) where T : PostBase;

        /// <summary>
        /// Gets the available posts for the specified blog.
        /// </summary>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The posts</returns>
        Task<IEnumerable<DynamicPost>> GetAllBySiteIdAsync(Guid? siteId = null);

        /// <summary>
        /// Gets the available post items.
        /// </summary>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The posts</returns>
        Task<IEnumerable<T>> GetAllBySiteIdAsync<T>(Guid? siteId = null) where T : PostBase;

        /// <summary>
        /// Gets the available posts for the specified blog.
        /// </summary>
        /// <param name="slug">The blog slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The posts</returns>
        Task<IEnumerable<DynamicPost>> GetAllAsync(string slug, Guid? siteId = null);

        /// <summary>
        /// Gets the available posts for the specified blog.
        /// </summary>
        /// <param name="slug">The blog slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The posts</returns>
        Task<IEnumerable<T>> GetAllAsync<T>(string slug, Guid? siteId = null) where T : PostBase;

        /// <summary>
        /// Gets all available categories for the specified blog.
        /// </summary>
        /// <param name="id">The blog id</param>
        /// <returns>The available categories</returns>
        Task<IEnumerable<Taxonomy>> GetAllCategoriesAsync(Guid blogId);

        /// <summary>
        /// Gets all available tags for the specified blog.
        /// </summary>
        /// <param name="id">The blog id</param>
        /// <returns>The available tags</returns>
        Task<IEnumerable<Taxonomy>> GetAllTagsAsync(Guid blogId);

        /// <summary>
        /// Gets the id of all posts that have a draft for
        /// the specified blog.
        /// </summary>
        /// <param name="blogId">The unique blog id</param>
        /// <returns>The posts that have a draft</returns>
        Task<IEnumerable<Guid>> GetAllDraftsAsync(Guid blogId);

        /// <summary>
        /// Gets the number of available posts in the specified archive.
        /// </summary>
        /// <param name="archiveId">The archive id</param>
        /// <returns>The number of posts</returns>
        Task<int> GetCountAsync(Guid archiveId);

        /// <summary>
        /// Gets the post model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The post model</returns>
        Task<DynamicPost> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets the post model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <returns>The post model</returns>
        Task<T> GetByIdAsync<T>(Guid id) where T : PostBase;

        /// <summary>
        /// Gets the draft for the post model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <returns>The draft, or null if no draft exists</returns>
        Task<DynamicPost> GetDraftByIdAsync(Guid id);

        /// <summary>
        /// Gets the draft for the post model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <returns>The draft, or null if no draft exists</returns>
        Task<T> GetDraftByIdAsync<T>(Guid id) where T : PostBase;

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <param name="blog">The unique blog slug</param>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The post model</returns>
        Task<DynamicPost> GetBySlugAsync(string blog, string slug, Guid? siteId = null);

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="blog">The unique blog slug</param>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The post model</returns>
        Task<T> GetBySlugAsync<T>(string blog, string slug, Guid? siteId = null) where T : PostBase;

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <param name="blog">The unique blog slug</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The post model</returns>
        Task<DynamicPost> GetBySlugAsync(Guid blogId, string slug);

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="blog">The unique blog slug</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The post model</returns>
        Task<T> GetBySlugAsync<T>(Guid blogId, string slug) where T : PostBase;

        /// <summary>
        /// Gets the category with the id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The model</returns>
        Task<Taxonomy> GetCategoryByIdAsync(Guid id);

        /// <summary>
        /// Gets the category with the given slug.
        /// </summary>
        /// <param name="blogId">The blog id</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The model</returns>
        Task<Taxonomy> GetCategoryBySlugAsync(Guid blogId, string slug);

        /// <summary>
        /// Gets the tag with the id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The model</returns>
        Task<Taxonomy> GetTagByIdAsync(Guid id);

        /// <summary>
        /// Gets the tag with the given slug.
        /// </summary>
        /// <param name="blogId">The blog id</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The model</returns>
        Task<Taxonomy> GetTagBySlugAsync(Guid blogId, string slug);

        /// <summary>
        /// Saves the given post model
        /// </summary>
        /// <param name="model">The post model</param>
        Task SaveAsync<T>(T model) where T : PostBase;

        /// <summary>
        /// Saves the given post model as a draft
        /// </summary>
        /// <param name="model">The post model</param>
        Task SaveDraftAsync<T>(T model) where T : PostBase;

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        Task DeleteAsync<T>(T model) where T : PostBase;
    }
}
