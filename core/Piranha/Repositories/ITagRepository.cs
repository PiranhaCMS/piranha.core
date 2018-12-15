/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System;
using System.Collections.Generic;
using Piranha.Data;

namespace Piranha.Repositories
{
    public interface ITagRepository
    {
        /// <summary>
        /// Gets all available models.
        /// </summary>
        /// <param name="blogId">The blog id</param>
        /// <returns>The available models</returns>
        IEnumerable<Tag> GetAll(Guid blogId);

        /// <summary>
        /// Gets the models for the post with the given id.
        /// </summary>
        /// <param name="postId">The post id</param>
        /// <returns>The model</returns>
        IEnumerable<Tag> GetByPostId(Guid postId);        

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The model, or NULL if it doesn't exist</returns>
        Tag GetById(Guid id);

        /// <summary>
        /// Gets the model with the given slug
        /// </summary>
        /// <param name="blogId">The blog id</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The model</returns>
        Tag GetBySlug(Guid blogId, string slug);

        /// <summary>
        /// Gets the model with the given title
        /// </summary>
        /// <param name="blogId">The blog id</param>
        /// <param name="title">The unique title</param>
        /// <returns>The model</returns>
        Tag GetByTitle(Guid blogId, string title);

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        void Save(Tag model);

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        void Delete(Guid id);

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        void Delete(Tag model);

        /// <summary>
        /// Deletes all unused tags for the specified blog.
        /// </summary>
        /// <param name="blogId">The blog id</param>
        void DeleteUnused(Guid blogId);
    }
}
