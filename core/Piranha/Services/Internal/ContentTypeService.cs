/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.ComponentModel.DataAnnotations;
using Piranha.Cache;
using Piranha.Models;
using Piranha.Repositories;

namespace Piranha.Services;

internal sealed class ContentTypeService : IContentTypeService
{
    private readonly IContentTypeRepository _repo;
    private readonly ICache _cache;
    private static readonly string CacheKey = "Piranha_ContentTypes";

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="repo">The main repository</param>
    /// <param name="cache">The optional model cache</param>
    public ContentTypeService(IContentTypeRepository repo, ICache cache = null)
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
    public async Task<IEnumerable<ContentType>> GetAllAsync()
    {
        var types = await GetTypes().ConfigureAwait(false);
        return types ?? await _repo.GetAll().ConfigureAwait(false);
    }

    /// <summary>
    /// Gets all available models from the specified group.
    /// </summary>
    /// <param name="group">The content group</param>
    /// <returns>The available models</returns>
    public async Task<IEnumerable<ContentType>> GetByGroupAsync(string group)
    {
        var types = await GetTypes().ConfigureAwait(false);

        if (types != null)
        {
            return types.Where(t => t.Group == group).ToList();
        }
        return await _repo.GetByGroup(group).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <returns>The model</returns>
    public async Task<ContentType> GetByIdAsync(string id)
    {
        var types = await GetTypes().ConfigureAwait(false);

        if (types != null)
        {
            return types.FirstOrDefault(t => t.Id == id);
        }
        return await _repo.GetById(id).ConfigureAwait(false);
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

        // Validate regions
        foreach (var region in model.Regions)
        {
            var regionContext = new ValidationContext(region);
            Validator.ValidateObject(region, regionContext, true);

            // Validate fields
            foreach (var field in region.Fields)
            {
                var fieldContext = new ValidationContext(field);
                Validator.ValidateObject(field, fieldContext, true);
            }
        }

        // Call hooks & save
        App.Hooks.OnBeforeSave(model);
        await _repo.Save(model).ConfigureAwait(false);
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
    public async Task DeleteAsync(ContentType model)
    {
        // Call hooks & delete
        App.Hooks.OnBeforeDelete(model);
        await _repo.Delete(model.Id).ConfigureAwait(false);
        App.Hooks.OnAfterDelete(model);

        // Clear cache
        _cache?.Remove(CacheKey);
    }

    /// <summary>
    /// Deletes the given models.
    /// </summary>
    /// <param name="models">The models</param>
    public async Task DeleteAsync(IEnumerable<ContentType> models)
    {
        if (models != null && models.Count() > 0)
        {
            foreach (var model in models)
            {
                // Call hooks & delete
                App.Hooks.OnBeforeDelete(model);
                await _repo.Delete(model.Id).ConfigureAwait(false);
                App.Hooks.OnAfterDelete(model);
            }
            // Clear cache
            _cache?.Remove(CacheKey);
        }
    }

    /// <summary>
    /// Gets the content types from the database.
    /// </summary>
    private async Task<IEnumerable<ContentType>> GetTypes()
    {
        if (_cache != null)
        {
            var types = _cache.Get<IEnumerable<ContentType>>(CacheKey);

            if (types == null)
            {
                types = await _repo.GetAll().ConfigureAwait(false);

                _cache.Set(CacheKey, types);
            }
            return types;
        }
        return null;
    }
}
