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

internal sealed class ContentGroupService : IContentGroupService
{
    private readonly IContentGroupRepository _repo;
    private readonly ICache _cache;
    private static readonly string CacheKey = "Piranha_ContentGroup";

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="repo">The main repository</param>
    /// <param name="cache">The optional model cache</param>
    public ContentGroupService(IContentGroupRepository repo, ICache cache = null)
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
    public async Task<IEnumerable<ContentGroup>> GetAllAsync()
    {
        var groups = await GetGroups().ConfigureAwait(false);
        return groups ?? await _repo.GetAllAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <returns>The model</returns>
    public async Task<ContentGroup> GetByIdAsync(string id)
    {
        var groups = await GetGroups().ConfigureAwait(false);

        if (groups != null)
        {
            return groups.FirstOrDefault(t => t.Id == id);
        }
        return await _repo.GetByIdAsync(id).ConfigureAwait(false);
    }

    /// <summary>
    /// Adds or updates the given model in the database
    /// depending on its state.
    /// </summary>
    /// <param name="model">The model</param>
    public async Task SaveAsync(ContentGroup model)
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
    public async Task DeleteAsync(ContentGroup model)
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
    private async Task<IEnumerable<ContentGroup>> GetGroups()
    {
        if (_cache != null) {
            var groups = _cache.Get<IEnumerable<ContentGroup>>(CacheKey);

            if (groups == null)
            {
                groups = await _repo.GetAllAsync().ConfigureAwait(false);

                _cache.Set(CacheKey, groups);
            }
            return groups;
        }
        return null;
    }
}
