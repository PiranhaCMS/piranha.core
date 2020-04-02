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
    public static class PostTypeServiceSyncExtensions
    {
        /// <summary>
        /// Gets all available models.
        /// </summary>
        /// <returns>The available models</returns>
        [Obsolete]
        public static IEnumerable<PostType> GetAll(this IPostTypeService service)
        {
            return service.GetAllAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="service">The post type service</param>
        /// <param name="id">The unique i</param>
        /// <returns></returns>
        [Obsolete]
        public static PostType GetById(this IPostTypeService service, string id)
        {
            return service.GetByIdAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="service">The post type service</param>
        /// <param name="model">The model</param>
        [Obsolete]
        public static void Save(this IPostTypeService service, PostType model)
        {
            service.SaveAsync(model).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="service">The post type service</param>
        /// <param name="id">The unique id</param>
        [Obsolete]
        public static void Delete(this IPostTypeService service, string id)
        {
            service.DeleteAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="service">The post type service</param>
        /// <param name="model">The model</param>
        [Obsolete]
        public static void Delete(this IPostTypeService service, PostType model)
        {
            service.DeleteAsync(model).GetAwaiter().GetResult();
        }
    }
}
