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
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading.Tasks;
using Piranha.Models;
using Piranha.Repositories;

namespace Piranha.Services
{
    public class ContentService : IContentService
    {
        private readonly IContentRepository _pageRepo;
        private readonly IContentFactory _factory;
        private readonly ILanguageService _langService;
        private readonly ICache _cache;
        private readonly ISearch _search;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="pageRepo">The main page repository</param>
        /// <param name="factory">The content factory</param>
        /// <param name="langService">The language service</param>
        /// <param name="cache">The optional cache service</param>
        /// <param name="search">The optional search service</param>
        public ContentService(IContentRepository pageRepo, IContentFactory factory, ILanguageService langService, ICache cache = null, ISearch search = null)
        {
            _pageRepo = pageRepo;
            _factory = factory;
            _langService = langService;

            if ((int)App.CacheLevel > 2)
            {
                _cache = cache;
            }
            _search = search;
        }

        /// <summary>
        /// Creates and initializes a new content model.
        /// </summary>
        /// <param name="typeId">The content type id</param>
        /// <returns>The created page</returns>
        public async Task<T> CreateAsync<T>(string typeId) where T : GenericContent
        {
            if (string.IsNullOrEmpty(typeId))
            {
                typeId = typeof(T).Name;
            }

            var type = App.ContentTypes.GetById(typeId);

            if (type != null)
            {
                var model = await _factory.CreateAsync<T>(type).ConfigureAwait(false);

                return model;
            }
            return null;
        }

        /// <summary>
        /// Gets the content model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <param name="languageId">The optional language id</param>
        /// <returns>The content model</returns>
        public async Task<T> GetByIdAsync<T>(Guid id, Guid? languageId = null) where T : GenericContent
        {
            T model = null;

            // Make sure we have a language id
            if (languageId == null)
            {
                languageId = (await _langService.GetDefaultAsync())?.Id;
            }

            // First, try to get the model from cache
            if (typeof(IDynamicContent).IsAssignableFrom(typeof(T)))
            {
                model = null; // TODO: _cache?.Get<T>($"DynamicContent_{ id.ToString() }");
            }
            else
            {
                model = null; // TODO: _cache?.Get<T>(id.ToString());
            }

            // If we have a model, let's initialize it
            if (model != null)
            {
                await _factory.InitAsync(model, App.ContentTypes.GetById(model.TypeId)).ConfigureAwait(false);
            }

            // If we don't have a model, get it from the repository
            if (model == null)
            {
                model = await _pageRepo.GetById<T>(id, languageId.Value).ConfigureAwait(false);

                await OnLoadAsync(model).ConfigureAwait(false);
            }

            // Check that we got back the requested type from the
            // repository
            if (model != null && model is T)
            {
                return model;
            }
            return null;
        }

        /// <summary>
        /// Saves the given content model
        /// </summary>
        /// <param name="model">The content model</param>
        /// <param name="languageId">The optional language id</param>
        public async Task SaveAsync<T>(T model, Guid? languageId = null) where T : GenericContent
        {
            // Make sure we have an Id
            if (model.Id == Guid.Empty)
            {
                model.Id = Guid.NewGuid();
            }

            // Make sure we have a language id
            if (languageId == null)
            {
                languageId = (await _langService.GetDefaultAsync())?.Id;
            }

            // Validate model
            var context = new ValidationContext(model);
            Validator.ValidateObject(model, context, true);

            // Ensure category
            if (model is ICategorizedContent categorizedModel)
            {
                if (categorizedModel.Category == null || (string.IsNullOrWhiteSpace(categorizedModel.Category.Title) && string.IsNullOrWhiteSpace(categorizedModel.Category.Slug)))
                {
                    throw new ValidationException("The Category field is required");
                }
            }

            // Call hooks and save
            App.Hooks.OnBeforeSave<GenericContent>(model);
            await _pageRepo.Save(model, languageId.Value);
            App.Hooks.OnAfterSave<GenericContent>(model);

            // Remove from cache
            await RemoveFromCacheAsync(model).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the content model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task DeleteAsync(Guid id)
        {
            var model = await GetByIdAsync<GenericContent>(id).ConfigureAwait(false);

            if (model != null)
            {
                await DeleteAsync(model).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Deletes the given content model.
        /// </summary>
        /// <param name="model">The content model</param>
        public async Task DeleteAsync<T>(T model) where T : GenericContent
        {
            // Call hooks and delete
            App.Hooks.OnBeforeDelete<GenericContent>(model);
            await _pageRepo.Delete(model.Id).ConfigureAwait(false);
            App.Hooks.OnAfterDelete<GenericContent>(model);

            // Delete search document
            if (_search != null)
            {
                // TODO
                // await _search.DeletePageAsync(model);
            }

            // Remove from cache
            await RemoveFromCacheAsync(model).ConfigureAwait(false);
        }

        /// <summary>
        /// Processes the model after it has been loaded from
        /// the repository.
        /// </summary>
        /// <param name="model">The content model</param>
        private async Task OnLoadAsync(GenericContent model)
        {
            // Make sure we have a model
            if (model == null) return;

            // Initialize the model
            await _factory.InitAsync(model, App.ContentTypes.GetById(model.TypeId));

            // Execute on load hook
            App.Hooks.OnLoad(model);

            // Update the cache if available
            if (_cache != null)
            {
                // Store the model
                if (model is IDynamicContent)
                {
                    _cache.Set($"DynamicContent_{ model.Id.ToString() }", model);
                }
                else
                {
                    _cache.Set(model.Id.ToString(), model);
                }
            }
        }

        /// <summary>
        /// Removes the given model from the cache.
        /// </summary>
        /// <param name="model">The model</param>
        private Task RemoveFromCacheAsync(GenericContent model)
        {
            return Task.Run(() =>
            {
                if (_cache != null)
                {
                    _cache.Remove(model.Id.ToString());
                    _cache.Remove($"DynamicContent_{ model.Id.ToString() }");
                }
            });
        }
    }
}
