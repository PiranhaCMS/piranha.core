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

namespace Piranha.Repositories
{
    public interface IPostRepository
    {
        /// <summary>
        /// Gets the available post items.
        /// </summary>
        /// <returns>The posts</returns>
        IEnumerable<Models.DynamicPost> GetAll();

        /// <summary>
        /// Gets the post model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The post model</returns>
        Models.DynamicPost GetById(Guid id);

        /// <summary>
        /// Gets the post model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <returns>The post model</returns>
        T GetById<T>(Guid id) where T : Models.Post<T>;

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <param name="category">The unique category slug</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The post model</returns>
        Models.DynamicPost GetBySlug(string category, string slug);

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="categorySlug">The unique category slug</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The post model</returns>
        T GetBySlug<T>(string categorySlug, string slug) where T : Models.Post<T>;

        /// <summary>
        /// Gets the available post items for the given category id.
        /// </summary>
        /// <param name="id">The unique category id</param>
        /// <returns>The posts</returns>
        IList<Models.DynamicPost> GetByCategoryId(Guid id);

        /// <summary>
        /// Gets the available post items for the given category slug.
        /// </summary>
        /// <param name="slug">The unique category slug</param>
        /// <returns>The posts</returns>
        IList<Models.DynamicPost> GetByCategorySlug(string slug);

        /// <summary>
        /// Saves the given post model
        /// </summary>
        /// <param name="model">The post model</param>
        void Save<T>(T model) where T : Models.Post<T>;

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        void Delete(Guid id);

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        void Delete<T>(T model) where T : Models.Post<T>;
    }
}
