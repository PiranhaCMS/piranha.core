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
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Piranha.Models;
using Piranha.Repositories;

namespace Piranha.Services
{
    public class ParamService : IParamService
    {
        private readonly IParamRepository _repo;
        private readonly ICache _cache;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="repo">The main repository</param>
        /// <param name="cache">The optional model cache</param>
        public ParamService(IParamRepository repo, ICache cache = null)
        {
            _repo = repo;

            if ((int)App.CacheLevel > 0)
            {
                _cache = cache;
            }
        }

        /// <summary>
        /// Gets all available models.
        /// </summary>
        /// <returns>The available models</returns>
        public Task<IEnumerable<Param>> GetAllAsync()
        {
            return _repo.GetAll();
        }

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The model, or null if it doesn't exist</returns>
        public async Task<Param> GetByIdAsync(Guid id)
        {
            var model = _cache?.Get<Param>(id.ToString());

            if (model == null)
            {
                model = await _repo.GetById(id).ConfigureAwait(false);

                OnLoad(model);
            }
            return model;
        }

        /// <summary>
        /// Gets the model with the given key.
        /// </summary>
        /// <param name="key">The unique key</param>
        /// <returns>The model</returns>
        public async Task<Param> GetByKeyAsync(string key) {
            var id = _cache?.Get<Guid?>($"ParamKey_{key}");
            Param model = null;

            if (id.HasValue)
            {
                model = await GetByIdAsync(id.Value).ConfigureAwait(false);
            }
            else
            {
                model = await _repo.GetByKey(key).ConfigureAwait(false);

                OnLoad(model);
            }
            return model;
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        public async Task SaveAsync(Param model)
        {
            // Ensure id
            if (model.Id == Guid.Empty)
            {
                model.Id = Guid.NewGuid();
            }

            // Validate model
            var context = new ValidationContext(model);
            Validator.ValidateObject(model, context, true);

            // Ensure key uniqueness
            var param = await _repo.GetByKey(model.Key).ConfigureAwait(false);
            if (param != null && param.Id != model.Id)
            {
                throw new ValidationException($"The Key field must be unique");
            }

            // Call hooks & save
            App.Hooks.OnBeforeSave(model);
            await _repo.Save(model).ConfigureAwait(false);
            App.Hooks.OnAfterSave(model);

            // Remove from cache
            RemoveFromCache(model);
        }

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task DeleteAsync(Guid id)
        {
            var model = await GetByIdAsync(id).ConfigureAwait(false);

            if (model != null)
            {
                await DeleteAsync(model).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        public async Task DeleteAsync(Param model)
        {
            // Call hooks & delete
            App.Hooks.OnBeforeDelete(model);
            await _repo.Delete(model.Id).ConfigureAwait(false);
            App.Hooks.OnAfterDelete(model);

            // Remove from cache
            RemoveFromCache(model);
        }

        /// <summary>
        /// Processes the model on load.
        /// </summary>
        /// <param name="model">The model</param>
        private void OnLoad(Param model)
        {
            if (model != null)
            {
                App.Hooks.OnLoad(model);

                if (_cache != null)
                {
                    _cache.Set(model.Id.ToString(), model);
                    _cache.Set($"ParamKey_{model.Key}", model.Id);
                }
            }
        }

        /// <summary>
        /// Removes the given model from cache.
        /// </summary>
        /// <param name="model">The model</param>
        private void RemoveFromCache(Param model) {
            if (_cache != null)
            {
                _cache.Remove(model.Id.ToString());
                _cache.Remove($"ParamKey_{model.Key}");
            }
        }
    }
}
