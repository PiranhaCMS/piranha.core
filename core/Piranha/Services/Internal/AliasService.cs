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

internal sealed class AliasService : IAliasService
{
    internal class AliasUrlCacheEntry
    {
        public Guid Id { get; set; }
        public string AliasUrl { get; set; }
    }

    private readonly IAliasRepository _repo;
    private readonly ISiteService _siteService;
    private readonly ICache _cache;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="repo">The main repository</param>
    /// <param name="siteService">The site service</param>
    /// <param name="cache">The optional model cache</param>
    public AliasService(IAliasRepository repo, ISiteService siteService, ICache cache = null)
    {
        _repo = repo;
        _siteService = siteService;

        if ((int)App.CacheLevel > 1)
        {
            _cache = cache;
        }
    }

    /// <summary>
    /// Gets all available models for the specified site.
    /// </summary>
    /// <param name="siteId">The optional site id</param>
    /// <returns>The available models</returns>
    public async Task<IEnumerable<Alias>> GetAllAsync(Guid? siteId = null)
    {
        if (!siteId.HasValue)
        {
            var site = await _siteService.GetDefaultAsync().ConfigureAwait(false);
            if (site != null)
            {
                siteId = site.Id;
            }
        }

        if (siteId.HasValue)
        {
            return await _repo.GetAll(siteId.Value).ConfigureAwait(false);
        }
        return null;
    }

    /// <summary>
    /// Gets the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The model, or null if it doesn't exist</returns>
    public async Task<Alias> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var model = _cache == null ? null : await _cache.GetAsync<Alias>(id.ToString(), cancellationToken).ConfigureAwait(false);

        if (model == null)
        {
            model = await _repo.GetById(id).ConfigureAwait(false);

            await OnLoad(model, cancellationToken).ConfigureAwait(false);
        }
        return model;
    }

    /// <summary>
    /// Gets the model with the given alias url.
    /// </summary>
    /// <param name="url">The unique url</param>
    /// <param name="siteId">The optional site id</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The model</returns>
    public async Task<Alias> GetByAliasUrlAsync(string url, Guid? siteId = null, CancellationToken cancellationToken = default)
    {
        if (!siteId.HasValue)
        {
            var site = await _siteService.GetDefaultAsync().ConfigureAwait(false);
            if (site != null)
            {
                siteId = site.Id;
            }
        }

        if (siteId.HasValue)
        {
            var aliasUrls = await GetAliasUrls(siteId.Value, cancellationToken).ConfigureAwait(false);

            if (aliasUrls != null)
            {
                var aliasUrl = aliasUrls.FirstOrDefault(x => x.AliasUrl == url);

                if (aliasUrl != null)
                {
                    return await GetByIdAsync(aliasUrl.Id).ConfigureAwait(false);
                }
            }
            else
            {
                return await _repo.GetByAliasUrl(url, siteId.Value);
            }
        }
        return null;
    }

    /// <summary>
    /// Gets the model with the given redirect url.
    /// </summary>
    /// <param name="url">The unique url</param>
    /// <param name="siteId">The optional site id</param>
    /// <returns>The model</returns>
    public async Task<IEnumerable<Alias>> GetByRedirectUrlAsync(string url, Guid? siteId = null)
    {
        if (!siteId.HasValue)
        {
            var site = await _siteService.GetDefaultAsync().ConfigureAwait(false);
            if (site != null)
            {
                siteId = site.Id;
            }
        }
        return await _repo.GetByRedirectUrl(url, siteId.Value).ConfigureAwait(false);
    }

    /// <summary>
    /// Adds or updates the given model in the database
    /// depending on its state.
    /// </summary>
    /// <param name="model">The model</param>
    /// <param name="cancellationToken"></param>
    public async Task SaveAsync(Alias model, CancellationToken cancellationToken)
    {
        // Ensure id
        if (model.Id == Guid.Empty)
        {
            model.Id = Guid.NewGuid();
        }

        // Validate model
        var context = new ValidationContext(model);
        Validator.ValidateObject(model, context, true);

        // Fix urls
        if (!model.AliasUrl.StartsWith("/"))
        {
            model.AliasUrl = "/" + model.AliasUrl;
        }
        if (!model.RedirectUrl.StartsWith("/") && !model.RedirectUrl.StartsWith("http://") && !model.RedirectUrl.StartsWith("https://"))
        {
            model.RedirectUrl = "/" + model.RedirectUrl;
        }

        // Ensure url uniqueness
        var alias = await _repo.GetByAliasUrl(model.AliasUrl, model.SiteId).ConfigureAwait(false);
        if (alias != null && alias.Id != model.Id)
        {
            throw new ValidationException($"The AliasUrl field must be unique");
        }

        // Call hooks & save
        App.Hooks.OnBeforeSave(model);
        await _repo.Save(model).ConfigureAwait(false);
        App.Hooks.OnAfterSave(model);

        // Remove from cache
        await RemoveFromCache(model, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <param name="cancellationToken"></param>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var model = await GetByIdAsync(id).ConfigureAwait(false);

        if (model != null)
        {
            await DeleteAsync(model, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Deletes the given model.
    /// </summary>
    /// <param name="model">The model</param>
    /// <param name="cancellationToken"></param>
    public async Task DeleteAsync(Alias model, CancellationToken cancellationToken = default)
    {
        // Call hooks & delete
        App.Hooks.OnBeforeDelete(model);
        await _repo.Delete(model.Id).ConfigureAwait(false);
        App.Hooks.OnAfterDelete(model);

        // Remove from cache
        await RemoveFromCache(model, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Processes the model on load.
    /// </summary>
    /// <param name="model">The model</param>
    /// <param name="cancellationToken"></param>
    private Task OnLoad(Alias model, CancellationToken cancellationToken)
    {
        if (model != null)
        {
            App.Hooks.OnLoad(model);

            if (_cache != null)
            {
                return _cache.SetAsync(model.Id.ToString(), model, cancellationToken);
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Removes the given model from cache.
    /// </summary>
    /// <param name="model">The model</param>
    /// <param name="cancellationToken"></param>
    private async Task RemoveFromCache(Alias model, CancellationToken cancellationToken)
    {
        if (_cache != null)
        {
            await _cache.RemoveAsync(model.Id.ToString(), cancellationToken).ConfigureAwait(false);
            await _cache.RemoveAsync($"Piranha_AliasUrls_{model.SiteId}", cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Gets the aliases for the specified site.
    /// </summary>
    private async Task<IEnumerable<AliasUrlCacheEntry>> GetAliasUrls(Guid siteId, CancellationToken cancellationToken)
    {
        if (_cache != null)
        {
            var aliasUrls = await _cache.GetAsync<IEnumerable<AliasUrlCacheEntry>>($"Piranha_AliasUrls_{siteId}", cancellationToken).ConfigureAwait(false);

            if (aliasUrls == null)
            {
                var aliases = await _repo.GetAll(siteId).ConfigureAwait(false);
                aliasUrls = aliases.Select(x => new AliasUrlCacheEntry
                {
                    Id = x.Id,
                    AliasUrl = x.AliasUrl
                }).ToList();

                await _cache.SetAsync($"Piranha_AliasUrls_{siteId}", aliasUrls, cancellationToken).ConfigureAwait(false);
            }
            return aliasUrls;
        }
        return null;
    }

}
