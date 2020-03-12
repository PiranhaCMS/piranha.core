/*
 * Copyright (c) 2020 Piranha CMS
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Piranha.Cache;
using Piranha.Models;
using Piranha.Repositories;

namespace Piranha.Services
{
    public class ContentTypeService : IContentTypeService
    {
        private readonly IContentTypeRepository _repo;
        private readonly ICache _cache;
        private static readonly string CacheKey = "Piranha_ContentTypes";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="repo">The main repository</param>
        /// <param name="cache">The optional model cache</param>
        public ContentTypeService(IContentTypeRepository repo, ICache cache)
        {
            _repo = repo;

            if (App.CacheLevel != CacheLevel.None)
            {
                _cache = cache;
            }
        }

        /// <summary>
        /// Gets all available models.
        /// </summary>
        /// <returns>The available models</returns>
        public Task<IEnumerable<ContentType>> GetAllAsync()
        {
            return GetTypes();
        }

        /// <summary>
        /// Gets all available models from the specified group.
        /// </summary>
        /// <param name="group">The content group</param>
        /// <returns>The available models</returns>
        public async Task<IEnumerable<ContentType>> GetByGroupAsync(string group)
        {
            // Check if we have cache enabled
            if (_cache != null && App.CacheLevel != CacheLevel.None)
            {
                var types = await GetTypes().ConfigureAwait(false);

                return types.Where(t => t.Group == group).ToList();
            }
            else
            {
                return await _repo.GetByGroupAsync(group).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The model</returns>
        public async Task<ContentType> GetByIdAsync(string id)
        {
            if (_cache != null && App.CacheLevel != CacheLevel.None)
            {
                var types = await GetTypes().ConfigureAwait(false);

                return types.FirstOrDefault(t => t.Id == id);
            }
            else
            {
                return await _repo.GetByIdAsync(id).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        public async Task SaveAsync(ContentType model)
        {
            // Validate model
            var context = new ValidationContext(model);
            Validator.ValidateObject(model, context, true);

            // Call hooks & save
            App.Hooks.OnBeforeSave(model);
            await _repo.SaveAsync(model).ConfigureAwait(false);
            App.Hooks.OnAfterSave(model);

            // Clear cache
            _cache?.Remove(CacheKey);
        }

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task DeleteAsync(string id)
        {
            var model = await _repo.GetByIdAsync(id).ConfigureAwait(false);

            if (model != null)
            {
                await DeleteAsync(model).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        public async Task DeleteAsync(ContentType model)
        {
            // Call hooks & delete
            App.Hooks.OnBeforeDelete(model);
            await _repo.DeleteAsync(model.Id).ConfigureAwait(false);
            App.Hooks.OnAfterDelete(model);

            // Clear cache
            _cache?.Remove(CacheKey);
        }

        /// <summary>
        /// Gets the content types from the database.
        /// </summary>
        private async Task<IEnumerable<ContentType>> GetTypes()
        {
            var types = _cache?.Get<IEnumerable<ContentType>>(CacheKey);

            if (types == null)
            {
                types = await _repo.GetAllAsync().ConfigureAwait(false);

                _cache?.Set(CacheKey, types);
            }
            return types;
        }
    }
}
