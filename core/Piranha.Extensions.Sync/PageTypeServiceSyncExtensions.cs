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
    public static class PageTypeServiceSyncExtensions
    {
        /// <summary>
        /// Gets all available models.
        /// </summary>
        /// <returns>The available models</returns>
        public static IEnumerable<PageType> GetAll(this IPageTypeService service)
        {
            return service.GetAllAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="service">The page type service</param>
        /// <param name="id">The unique i</param>
        /// <returns></returns>
        public static Models.PageType GetById(this IPageTypeService service, string id)
        {
            return service.GetByIdAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="service">The page type service</param>
        /// <param name="model">The model</param>
        public static void Save(this IPageTypeService service, PageType model)
        {
            service.SaveAsync(model).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="service">The page type service</param>
        /// <param name="id">The unique id</param>
        public static void Delete(this IPageTypeService service, string id)
        {
            service.DeleteAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="service">The page type service</param>
        /// <param name="model">The model</param>
        public static void Delete(this IPageTypeService service, PageType model)
        {
            service.DeleteAsync(model).GetAwaiter().GetResult();
        }
    }
}
