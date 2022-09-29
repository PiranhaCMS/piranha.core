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
public interface ISiteHelper
{
    /// <summary>
    /// Gets/sets the id of the currently requested site.
    /// </summary>
    Guid Id { get; set; }

    /// <summary>
    /// Gets/sets the language id of the currently requested site.
    /// </summary>
    Guid LanguageId { get; set; }

    /// <summary>
    /// Gets/sets the optional culture of the requested site.
    /// </summary>
    string Culture { get; set; }

    /// <summary>
    /// Gets/set the optional hostname of the requested site.
    /// </summary>
    string Host { get; set; }

    /// <summary>
    /// Gets/sets the optional site prefix of the requested site
    /// if it's routed with `host/prefix`.
    /// </summary>
    string SitePrefix { get; set; }

    /// <summary>
    /// Gets/sets the sitemap of the currently requested site.
    /// </summary>
    Sitemap Sitemap { get; set; }

    /// <summary>
    /// Gets/sets the site description.
    /// </summary>
    SiteDescription Description { get; set; }

    /// <summary>
    /// Gets the site content for the current site.
    /// </summary>
    /// <typeparam name="T">The content type</typeparam>
    /// <returns>The site content model</returns>
    Task<T> GetContentAsync<T>() where T : SiteContent<T>;
}
