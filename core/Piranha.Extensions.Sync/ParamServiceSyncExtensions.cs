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
    public static class ParamServiceSyncExtensions
    {
        /// <summary>
        /// Gets all available models.
        /// </summary>
        /// <returns>The available models</returns>
        [Obsolete]
        public static IEnumerable<Param> GetAll(this IParamService service)
        {
            return service.GetAllAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="service">The param service</param>
        /// <param name="id">The unique id</param>
        /// <returns>The model, or null if it doesn't exist</returns>
        [Obsolete]
        public static Param GetById(this IParamService service, Guid id)
        {
            return service.GetByIdAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the model with the given key.
        /// </summary>
        /// <param name="service">The param service</param>
        /// <param name="key">The unique key</param>
        /// <returns>The model</returns>
        [Obsolete]
        public static Param GetByKey(this IParamService service, string key) {
            return service.GetByKeyAsync(key).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="service">The param service</param>
        /// <param name="model">The model</param>
        [Obsolete]
        public static void Save(this IParamService service, Param model)
        {
            service.SaveAsync(model).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="service">The param service</param>
        /// <param name="id">The unique id</param>
        [Obsolete]
        public static void Delete(this IParamService service, Guid id)
        {
            service.DeleteAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="service">The param service</param>
        /// <param name="model">The model</param>
        [Obsolete]
        public static void Delete(this IParamService service, Param model)
        {
            service.DeleteAsync(model).GetAwaiter().GetResult();
        }
    }
}
