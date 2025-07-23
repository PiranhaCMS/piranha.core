﻿/*
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

internal sealed class PostTypeService : IPostTypeService
{
    private readonly IPostTypeRepository _repo;
    private readonly ICache _cache;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="repo">The main repository</param>
    /// <param name="cache">The optional model cache</param>
    public PostTypeService(IPostTypeRepository repo, ICache cache = null)
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
    public async Task<IEnumerable<PostType>> GetAllAsync()
    {
        var types = await GetTypes().ConfigureAwait(false);
        return types ?? await _repo.GetAll().ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the model with the specified id.
    /// </summary>
    /// <param name="id">The unique i</param>
    /// <returns></returns>
    public async Task<PostType> GetByIdAsync(string id)
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
    public async Task SaveAsync(PostType model)
    {
        // Validate model
        var context = new ValidationContext(model);
        Validator.ValidateObject(model, context, true);

        // Call hooks & save
        App.Hooks.OnBeforeSave(model);
        await _repo.Save(model).ConfigureAwait(false);
        App.Hooks.OnAfterSave(model);

        // Clear cache
        if (_cache != null)
        {
            await _cache.RemoveAsync("Piranha_PostTypes").ConfigureAwait(false);
        }
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
    public async Task DeleteAsync(PostType model)
    {
        // Call hooks & delete
        App.Hooks.OnBeforeDelete(model);
        await _repo.Delete(model.Id).ConfigureAwait(false);
        App.Hooks.OnAfterDelete(model);

        // Clear cache
        if (_cache != null)
        {
            await _cache.RemoveAsync("Piranha_PostTypes").ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Deletes the given models.
    /// </summary>
    /// <param name="models">The models</param>
    public async Task DeleteAsync(IEnumerable<PostType> models)
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
            if (_cache != null)
            {
                await _cache.RemoveAsync("Piranha_PostTypes").ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Reloads the page types from the database.
    /// </summary>
    private async Task<IEnumerable<PostType>> GetTypes()
    {
        if (_cache != null)
        {
            var types = await _cache.GetAsync<IEnumerable<PostType>>("Piranha_PostTypes").ConfigureAwait(false);

            if (types == null)
            {
                types = await _repo.GetAll().ConfigureAwait(false);

                await _cache.SetAsync("Piranha_PostTypes", types).ConfigureAwait(false);
            }
            return types;
        }
        return null;
    }
}
