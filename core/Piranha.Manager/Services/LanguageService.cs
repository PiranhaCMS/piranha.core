/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Manager.Models;
using Piranha.Models;

namespace Piranha.Manager.Services;

public class LanguageService
{
    private readonly IApi _api;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="api">The current api</param>
    public LanguageService(IApi api)
    {
        _api = api;
    }

    /// <summary>
    /// Gets the edit model for the language modal.
    /// </summary>
    /// <returns>The edit model</returns>
    public async Task<LanguageEditModel> Get()
    {
        return new LanguageEditModel
        {
            Items = await _api.Languages.GetAllAsync()
        };
    }

    /// <summary>
    /// Saves the given language and returns the updated edit model.
    /// </summary>
    /// <param name="model">The language model</param>
    /// <returns>The updated data</returns>
    public async Task<LanguageEditModel> Save(Language model)
    {
        await _api.Languages.SaveAsync(model);

        return await Get();
    }

    /// <summary>
    /// Deletes the given language and returns the updated edit model.
    /// </summary>
    /// <param name="id">The id of the language to delete</param>
    /// <returns>The updated data</returns>
    public async Task<LanguageEditModel> Delete(Guid id)
    {
        var defaultLanguage = await _api.Languages.GetDefaultAsync();
        var sites = await _api.Sites.GetAllAsync();

        // Assign all sites to the default language that has
        // the specified language selected.
        foreach (var site in sites)
        {
            if (site.LanguageId == id)
            {
                site.LanguageId = defaultLanguage.Id;
                await _api.Sites.SaveAsync(site);
            }
        }
        // Now delete the language
        await _api.Languages.DeleteAsync(id);

        return await Get();
    }

    /// <summary>
    /// Saves the given edit model and returns the updated data.
    /// </summary>
    /// <param name="model">The edit model</param>
    /// <returns>The updated data</returns>
    public async Task<LanguageEditModel> Save(LanguageEditModel model)
    {
        var current = await _api.Languages.GetAllAsync();

        var removed = current
            .Where(c => !model.Items.Select(i => i.Id).Contains(c.Id))
            .Select(c => c.Id);
        var defaultLanguage = model.Items.Single(i => i.IsDefault);

        // First let's save the default language
        if (defaultLanguage.Id == Guid.Empty)
        {
            defaultLanguage.Id = Guid.NewGuid();
        }
        await _api.Languages.SaveAsync(defaultLanguage);

        // Now let's delete the removed languages
        foreach (var r in removed)
        {
            await _api.Languages.DeleteAsync(r);
        }

        // Let's save the rest of the languages
        foreach (var item in model.Items.Where(i => i.Id != defaultLanguage.Id))
        {
            await _api.Languages.SaveAsync(item);
        }
        return await Get();
    }
}
