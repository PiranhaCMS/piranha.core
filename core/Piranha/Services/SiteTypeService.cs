﻿/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Piranha.Models;
using Piranha.Repositories;

namespace Piranha.Services
{
    public class SiteTypeService : ISiteTypeService
    {
        private readonly ISiteTypeRepository _repo;
        private readonly ICache _cache;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="repo">The main repository</param>
        /// <param name="cache">The optional model cache</param>
        public SiteTypeService(ISiteTypeRepository repo, ICache cache)
        {
            _repo = repo;

            if ((int)App.CacheLevel > 1)
            {
                _cache = cache;
            }
        }

        /// <summary>
        /// Gets all available models.
        /// </summary>
        /// <returns>The available models</returns>
        public Task<IEnumerable<SiteType>> GetAllAsync()
        {
            return GetTypes();
        }

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="id">The unique i</param>
        /// <returns></returns>
        public async Task<SiteType> GetByIdAsync(string id)
        {
            var types = await GetTypes().ConfigureAwait(false);

            return types.FirstOrDefault(t => t.Id == id);
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        public async Task SaveAsync(SiteType model)
        {
            // Validate model
            var context = new ValidationContext(model);
            Validator.ValidateObject(model, context, true);

            // Call hooks & save
            App.Hooks.OnBeforeSave<SiteType>(model);
            await _repo.Save(model).ConfigureAwait(false);
            App.Hooks.OnAfterSave<SiteType>(model);

            // Clear cache
            _cache?.Remove("Piranha_SiteTypes");
        }

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task DeleteAsync(string id)
        {
            var model = await _repo.GetById(id).ConfigureAwait(false);

            if (model != null)
            {
                await DeleteAsync(model).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        public async Task DeleteAsync(SiteType model)
        {
            // Call hooks & delete
            App.Hooks.OnBeforeDelete<SiteType>(model);
            await _repo.Delete(model.Id).ConfigureAwait(false);
            App.Hooks.OnAfterDelete<SiteType>(model);

            // Clear cache
            _cache?.Remove("Piranha_SiteTypes");
        }

        /// <summary>
        /// Reloads the page types from the database.
        /// </summary>
        private async Task<IEnumerable<SiteType>> GetTypes()
        {
            var types = _cache?.Get<IEnumerable<SiteType>>("Piranha_SiteTypes");

            if (types == null)
            {
                types = await _repo.GetAll().ConfigureAwait(false);

                _cache?.Set("Piranha_SiteTypes", types);
            }
            return types;
        }
    }
}
