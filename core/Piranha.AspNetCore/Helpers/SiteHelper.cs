/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.AspNetCore.Models;
using Piranha.Models;

namespace Piranha.AspNetCore.Helpers;

/// <summary>
/// Helper for accessing information about site in
/// the current request.
/// </summary>
public class SiteHelper : ISiteHelper
{
    private readonly IApi _api;

    /// <summary>
    /// Gets the id of the currently requested site.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets/sets the language id of the currently requested site.
    /// </summary>
    public Guid LanguageId { get; set; }

    /// <summary>
    /// Gets/sets the optional culture of the requested site.
    /// </summary>
    public string Culture { get; set; }

    /// <summary>
    /// Gets/set the optional hostname of the requested site.
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// Gets/sets the optional site prefic of the requested site
    /// if it's routed with `host/prefix`.
    /// </summary>
    public string SitePrefix { get; set; }

    /// <summary>
    /// Gets the sitemap of the currently requested site.
    /// </summary>
    public Sitemap Sitemap { get; set; }

    /// <summary>
    /// Gets/sets the site description.
    /// </summary>
    public SiteDescription Description { get; set; } = new SiteDescription();

    /// <summary>
    /// Default internal constructur.
    /// </summary>
    internal SiteHelper(IApi api)
    {
        _api = api;
    }

    /// <summary>
    /// Gets the site content for the current site.
    /// </summary>
    /// <typeparam name="T">The content type</typeparam>
    /// <returns>The site content model</returns>
    public Task<T> GetContentAsync<T>() where T : SiteContent<T>
    {
        if (Id != Guid.Empty)
        {
            return _api.Sites.GetContentByIdAsync<T>(Id);
        }
        return null;
    }
}
