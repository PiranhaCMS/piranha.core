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
    /// The client post repository interface.
    /// </summary>
    public interface IPostRepository
    {
        /// <summary>
        /// Gets the post model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The post model</returns>
        Post GetById(Guid id);

        /// <summary>
        /// Gets the post model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <returns>The post model</returns>
        T GetById<T>(Guid id) where T : Post;

        /// <summary>
        /// Gets the post models that matches the given
        /// id array.
        /// </summary>
        /// <param name="id">The id array</param>
        /// <returns>The post models</returns>
        IList<Post> GetById(Guid[] id);

        /// <summary>
        /// Gets the post models that matches the given
        /// id array.
        /// </summary>
        /// <param name="id">The id array</param>
        /// <returns>The post models</returns>
        IList<T> GetById<T>(Guid[] id) where T : Post;

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <param name="category">The unique category slug</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The post model</returns>
        Post GetBySlug(string category, string slug);

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="category">The unique category slug</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The post model</returns>
        T GetBySlug<T>(string category, string slug) where T : Post;
    }
}
