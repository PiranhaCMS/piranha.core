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
    public static class AliasServiceSyncExtensions
    {
        /// <summary>
        /// Gets all available models for the specified site.
        /// </summary>
        /// <param name="service">The alias service</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The available models</returns>
        [Obsolete]
        public static IEnumerable<Alias> GetAll(this IAliasService service, Guid? siteId = null)
        {
            return service.GetAllAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="service">The alias service</param>
        /// <param name="id">The unique id</param>
        /// <returns>The model, or null if it doesn't exist</returns>
        [Obsolete]
        public static Alias GetById(this IAliasService service, Guid id)
        {
            return service.GetByIdAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the model with the given alias url.
        /// </summary>
        /// <param name="service">The alias service</param>
        /// <param name="url">The unique url</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The model</returns>
        [Obsolete]
        public static Alias GetByAliasUrl(this IAliasService service, string url, Guid? siteId = null)
        {
            return service.GetByAliasUrlAsync(url, siteId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the model with the given redirect url.
        /// </summary>
        /// <param name="service">The alias service</param>
        /// <param name="url">The unique url</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The model</returns>
        [Obsolete]
        public static IEnumerable<Alias> GetByRedirectUrl(this IAliasService service, string url, Guid? siteId = null)
        {
            return service.GetByRedirectUrlAsync(url, siteId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="service">The alias service</param>
        /// <param name="model">The model</param>
        [Obsolete]
        public static void Save(this IAliasService service, Alias model)
        {
            service.SaveAsync(model).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="service">The alias service</param>
        /// <param name="id">The unique id</param>
        [Obsolete]
        public static void Delete(this IAliasService service, Guid id)
        {
            service.DeleteAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="service">The alias service</param>
        /// <param name="model">The model</param>
        [Obsolete]
        public static void Delete(this IAliasService service, Alias model)
        {
            service.DeleteAsync(model).GetAwaiter().GetResult();
        }
    }
}
