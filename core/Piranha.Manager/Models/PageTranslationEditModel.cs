/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Models;

namespace Piranha.Manager.Models;

/// <summary>
/// Represents one page being edited in all configured languages.
/// </summary>
public class PageTranslationEditModel
{
    /// <summary>
    /// Gets or sets the language-specific page models.
    /// </summary>
    public IList<PageEditModel> Pages { get; set; } = new List<PageEditModel>();

    /// <summary>
    /// Gets or sets the operation status.
    /// </summary>
    public StatusMessage Status { get; set; }
}