/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Extend.Fields;

namespace Piranha.AspNetCore.Models;

/// <summary>
/// Razor Page model for a single page.
/// </summary>
public class SiteDescription
{
    /// <summary>
    /// Gets/sets the current site title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets/sets the current site description.
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    /// Gets/sets the current site logo.
    /// </summary>
    public ImageField Logo { get; set; }
}
