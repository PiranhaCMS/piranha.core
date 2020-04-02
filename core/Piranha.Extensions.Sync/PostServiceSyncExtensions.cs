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
using Piranha.Models;

namespace Piranha.Services
{
    public static class PostServiceSyncExtensions
    {
        /// <summary>
        /// Gets the available posts for the specified blog.
        /// </summary>
        /// <param name="service">The post service</param>
        /// <param name="blogId">The unique blog id</param>
        /// <returns>The posts</returns>
        [Obsolete]
        public static IEnumerable<DynamicPost> GetAll(this IPostService service, Guid blogId)
        {
            return service.GetAllAsync(blogId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the available post items.
        /// </summary>
        /// <param name="service">The post service</param>
        /// <param name="blogId">The unique id</param>
        /// <returns>The posts</returns>
        [Obsolete]
        public static IEnumerable<T> GetAll<T>(this IPostService service, Guid blogId) where T : PostBase
        {
            return service.GetAllAsync<T>(blogId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the available posts for the specified blog.
        /// </summary>
        /// <param name="service">The post service</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The posts</returns>
        [Obsolete]
        public static IEnumerable<DynamicPost> GetAllBySiteId(this IPostService service, Guid? siteId = null)
        {
            return service.GetAllBySiteIdAsync(siteId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the available post items.
        /// </summary>
        /// <param name="service">The post service</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The posts</returns>
        [Obsolete]
        public static IEnumerable<T> GetAllBySiteId<T>(this IPostService service, Guid? siteId = null) where T : PostBase
        {
            return service.GetAllBySiteIdAsync<T>(siteId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the available posts for the specified blog.
        /// </summary>
        /// <param name="service">The post service</param>
        /// <param name="slug">The blog slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The posts</returns>
        [Obsolete]
        public static IEnumerable<DynamicPost> GetAll(this IPostService service, string slug, Guid? siteId = null)
        {
            return service.GetAllAsync(slug, siteId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the available posts for the specified blog.
        /// </summary>
        /// <param name="service">The post service</param>
        /// <param name="slug">The blog slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The posts</returns>
        [Obsolete]
        public static IEnumerable<T> GetAll<T>(this IPostService service, string slug, Guid? siteId = null) where T : PostBase
        {
            return service.GetAllAsync<T>(slug, siteId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets all available categories for the specified blog.
        /// </summary>
        /// <param name="service">The post service</param>
        /// <param name="blogId">The blog id</param>
        /// <returns>The available categories</returns>
        [Obsolete]
        public static IEnumerable<Taxonomy> GetAllCategories(this IPostService service, Guid blogId)
        {
            return service.GetAllCategoriesAsync(blogId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets all available tags for the specified blog.
        /// </summary>
        /// <param name="service">The post service</param>
        /// <param name="blogId">The blog id</param>
        /// <returns>The available tags</returns>
        [Obsolete]
        public static IEnumerable<Taxonomy> GetAllTags(this IPostService service, Guid blogId)
        {
            return service.GetAllTagsAsync(blogId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the post model with the specified id.
        /// </summary>
        /// <param name="service">The post service</param>
        /// <param name="id">The unique id</param>
        /// <returns>The post model</returns>
        [Obsolete]
        public static DynamicPost GetById(this IPostService service, Guid id)
        {
            return service.GetByIdAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the post model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="service">The post service</param>
        /// <param name="id">The unique id</param>
        /// <returns>The post model</returns>
        [Obsolete]
        public static T GetById<T>(this IPostService service, Guid id) where T : PostBase
        {
            return service.GetByIdAsync<T>(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <param name="service">The post service</param>
        /// <param name="blog">The unique blog slug</param>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The post model</returns>
        [Obsolete]
        public static DynamicPost GetBySlug(this IPostService service, string blog, string slug, Guid? siteId = null)
        {
            return service.GetBySlugAsync(blog, slug, siteId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="service">The post service</param>
        /// <param name="blog">The unique blog slug</param>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The post model</returns>
        [Obsolete]
        public static T GetBySlug<T>(this IPostService service, string blog, string slug, Guid? siteId = null) where T : PostBase
        {
            return service.GetBySlugAsync<T>(blog, slug, siteId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <param name="service">The post service</param>
        /// <param name="blogId">The unique blog slug</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The post model</returns>
        [Obsolete]
        public static DynamicPost GetBySlug(this IPostService service, Guid blogId, string slug)
        {
            return service.GetBySlugAsync(blogId, slug).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <param name="service">The post service</param>
        /// <param name="blogId">The unique blog slug</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The post model</returns>
        [Obsolete]
        public static T GetBySlug<T>(this IPostService service, Guid blogId, string slug) where T : PostBase
        {
            return service.GetBySlugAsync<T>(blogId, slug).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the category with the given slug.
        /// </summary>
        /// <param name="service">The post service</param>
        /// <param name="blogId">The blog id</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The model</returns>
        [Obsolete]
        public static Taxonomy GetCategoryBySlug(this IPostService service, Guid blogId, string slug)
        {
            return service.GetCategoryBySlugAsync(blogId, slug).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the tag with the given slug.
        /// </summary>
        /// <param name="service">The post service</param>
        /// <param name="blogId">The blog id</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The model</returns>
        [Obsolete]
        public static Taxonomy GetTagBySlug(this IPostService service, Guid blogId, string slug)
        {
            return service.GetTagBySlugAsync(blogId, slug).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Saves the given post model
        /// </summary>
        /// <param name="service">The post service</param>
        /// <param name="model">The post model</param>
        [Obsolete]
        public static void Save<T>(this IPostService service, T model) where T : PostBase
        {
            service.SaveAsync(model).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="service">The post service</param>
        /// <param name="id">The unique id</param>
        [Obsolete]
        public static void Delete(this IPostService service, Guid id)
        {
            service.DeleteAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="service">The post service</param>
        /// <param name="model">The model</param>
        [Obsolete]
        public static void Delete<T>(this IPostService service, T model) where T : PostBase
        {
            service.DeleteAsync(model).GetAwaiter().GetResult();
        }
    }
}
