/*
 * Copyright (c) 2016-2017 Håkan Edling
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
    public interface IPostRepository
    {
        /// <summary>
        /// Gets the available post items.
        /// </summary>
        /// <param name="blogId">The unique id</param>
        /// <returns>The posts</returns>
        IEnumerable<DynamicPost> GetAll(Guid blogId);

        /// <summary>
        /// Gets the available posts for the specified blog.
        /// </summary>
        /// <param name="slug">The blog slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The posts</returns>
        IEnumerable<DynamicPost> GetAll(string slug, Guid? siteId = null);

        /// <summary>
        /// Gets the post model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The post model</returns>
        DynamicPost GetById(Guid id);

        /// <summary>
        /// Gets the post model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <returns>The post model</returns>
        T GetById<T>(Guid id) where T : Post<T>;

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <param name="blog">The unique blog slug</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The post model</returns>
        DynamicPost GetBySlug(Guid blogId, string slug);

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="blog">The unique blog slug</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The post model</returns>
        T GetBySlug<T>(Guid blogId, string slug) where T : Post<T>;

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <param name="blog">The unique blog slug</param>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The post model</returns>
        DynamicPost GetBySlug(string blog, string slug, Guid? siteId = null);

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="blog">The unique blog slug</param>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The post model</returns>
        T GetBySlug<T>(string blog, string slug, Guid? siteId = null) where T : Post<T>;

        /// <summary>
        /// Saves the given post model
        /// </summary>
        /// <param name="model">The post model</param>
        void Save<T>(T model) where T : Post<T>;

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        void Delete(Guid id);

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        void Delete<T>(T model) where T : Post<T>;
    }
}
