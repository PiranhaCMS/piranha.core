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
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Piranha.Data;
using Piranha.Repositories;

namespace Piranha.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _repo;
        private readonly ICache _cache;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="db">The current db context</param>
        /// <param name="cache">The optional model cache</param>
        public CategoryService(ICategoryRepository repo, ICache cache = null)
        {
            _repo = repo;
            _cache = cache;
        }

        /// <summary>
        /// Gets all available models for the specified blog.
        /// </summary>
        /// <param name="id">The blog id</param>
        /// <returns>The available models</returns>
        public Task<IEnumerable<Category>> GetAllAsync(Guid blogId)
        {
            return _repo.GetAll(blogId);
        }

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The model, or null if it doesn't exist</returns>
        public async Task<Category> GetByIdAsync(Guid id)
        {
            var model = _cache?.Get<Category>(id.ToString());

            if (model == null)
            {
                model = await _repo.GetById(id);

                OnLoad(model);
            }
            return model;
        }

        /// <summary>
        /// Gets the model with the given slug.
        /// </summary>
        /// <param name="blogId">The blog id</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The model</returns>
        public async Task<Category> GetBySlugAsync(Guid blogId, string slug)
        {
            var id = _cache?.Get<Guid?>($"Category_{blogId}_{slug}");
            Category model = null;

            if (id.HasValue)
            {
                model = await GetByIdAsync(id.Value);
            }
            else
            {
                model = await _repo.GetBySlug(blogId, slug);

                OnLoad(model);
            }
            return model;
        }

        /// <summary>
        /// Gets the model with the given title
        /// </summary>
        /// <param name="blogId">The blog id</param>
        /// <param name="title">The unique title</param>
        /// <returns>The model</returns>
        public Task<Category> GetByTitleAsync(Guid blogId, string title)
        {
            return _repo.GetByTitle(blogId, title);
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        public async Task SaveAsync(Category model)
        {
            // Ensure id
            if (model.Id == Guid.Empty)
            {
                model.Id = Guid.NewGuid();
            }

            // Validate model
            var context = new ValidationContext(model);
            Validator.ValidateObject(model, context, true);

            // Ensure slug
            if (string.IsNullOrWhiteSpace(model.Slug))
            {
                model.Slug = Utils.GenerateSlug(model.Title, false);
            }
            else
            {
                model.Slug = Utils.GenerateSlug(model.Slug, false);
            }

            // Ensure slug uniqueness
            var category = await _repo.GetBySlug(model.BlogId, model.Slug);
            if (category != null && category.Id != model.Id)
            {
                throw new ValidationException($"The Slug field must be unique");
            }

            // Call hooks & save
            App.Hooks.OnBeforeSave<Category>(model);
            await _repo.Save(model);
            App.Hooks.OnAfterSave<Category>(model);

            // Remove from cache
            RemoveFromCache(model);
        }

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task DeleteAsync(Guid id)
        {
            var model = await GetByIdAsync(id);

            if (model != null)
            {
                await DeleteAsync(model);
            }
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        public async Task DeleteAsync(Category model)
        {
            // Call hooks & delete
            App.Hooks.OnBeforeDelete<Category>(model);
            await _repo.Delete(model.Id);
            App.Hooks.OnAfterDelete<Category>(model);

            // Remove from cache
            RemoveFromCache(model);
        }

        /// <summary>
        /// Deletes all unused categories for the specified blog.
        /// </summary>
        /// <param name="blogId">The blog id</param>
        public Task DeleteUnusedAsync(Guid blogId)
        {
            //
            // TODO: Handle events
            //
            return _repo.DeleteUnused(blogId);
        }

        /// <summary>
        /// Processes the model on load.
        /// </summary>
        /// <param name="model">The model</param>
        private void OnLoad(Category model)
        {
            if (model != null)
            {
                App.Hooks.OnLoad<Category>(model);

                if (_cache != null)
                {
                    _cache.Set(model.Id.ToString(), model);
                    _cache.Set($"Category_{model.BlogId}_{model.Slug}", model.Id);
                }
            }
        }

        /// <summary>
        /// Removes the given model from cache.
        /// </summary>
        /// <param name="model">The model</param>
        private void RemoveFromCache(Category model)
        {
            if (_cache != null)
            {
                _cache.Remove(model.Id.ToString());
                _cache.Remove($"Category_{model.BlogId}_{model.Slug}");
            }
        }
    }
}
