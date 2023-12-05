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

internal sealed class LanguageService : ILanguageService
{
    private readonly ILanguageRepository _repo;
    private readonly ICache _cache;
    private static readonly string CacheKey = "Piranha_Languages";

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="repo">The main repository</param>
    /// <param name="cache">The optional model cache</param>
    public LanguageService(ILanguageRepository repo, ICache cache = null)
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
    public async Task<IEnumerable<Language>> GetAllAsync()
    {
        var languages = await GetLanguages().ConfigureAwait(false);
        return languages ?? await _repo.GetAll().ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <returns>The model</returns>
    public async Task<Language> GetByIdAsync(Guid id)
    {
        var languages = await GetLanguages().ConfigureAwait(false);

        if (languages != null)
        {
            return languages.FirstOrDefault(t => t.Id == id);
        }
        return await _repo.GetById(id).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the default side.
    /// </summary>
    /// <returns>The modell</returns>
    public async Task<Language> GetDefaultAsync()
    {
        var languages = await GetLanguages().ConfigureAwait(false);

        if (languages != null)
        {
            return languages.FirstOrDefault(l => l.IsDefault);
        }
        return await _repo.GetDefault().ConfigureAwait(false);
    }

    /// <summary>
    /// Adds or updates the given model in the database
    /// depending on its state.
    /// </summary>
    /// <param name="model">The model</param>
    public async Task SaveAsync(Language model)
    {
        // Make sure we have an id
        if (model.Id == Guid.Empty)
        {
            model.Id = Guid.NewGuid();
        }

        // Validate model
        var context = new ValidationContext(model);
        Validator.ValidateObject(model, context, true);

        // Make sure we have a default language
        if (model.IsDefault)
        {
            // Make sure no other site is default first
            var def = await GetDefaultAsync().ConfigureAwait(false);

            if (def != null && def.Id != model.Id)
            {
                def.IsDefault = false;
                await _repo.Save(def).ConfigureAwait(false);
            }
        }
        else
        {
            // Make sure we have a default site
            var def = await _repo.GetDefault().ConfigureAwait(false);
            if (def == null || def.Id == model.Id)
                model.IsDefault = true;
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
    public async Task DeleteAsync(Guid id)
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
    public async Task DeleteAsync(Language model)
    {
        // Call hooks & delete
        App.Hooks.OnBeforeDelete(model);
        await _repo.Delete(model.Id).ConfigureAwait(false);
        App.Hooks.OnAfterDelete(model);

        // Clear cache
        _cache?.Remove(CacheKey);
    }

    /// <summary>
    /// Gets the languages from the database.
    /// </summary>
    private async Task<IEnumerable<Language>> GetLanguages()
    {
        if (_cache != null)
        {
            var types = _cache.Get<IEnumerable<Language>>(CacheKey);

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
