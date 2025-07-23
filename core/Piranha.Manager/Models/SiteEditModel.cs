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
using Piranha.Models;
using Piranha.Manager.Models.Content;

namespace Piranha.Manager.Models;

/// <summary>
/// Site model.
/// </summary>
public class SiteEditModel : Content.ContentEditModel
{
    /// <summary>
    /// Gets/sets the selected language id.
    /// </summary>
    public Guid LanguageId { get; set; }

    /// <summary>
    /// Gets/sets the internal textual id.
    /// </summary>
    public string InternalId { get; set; }

    /// <summary>
    /// Gets/sets the optional description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets/sets the optional site logo.
    /// </summary>
    public ImageField Logo { get; set; }

    /// <summary>
    /// Gets/sets the optional hostnames to bind this site for.
    /// </summary>
    public string Hostnames { get; set; }

    /// <summary>
    /// Gets/sets if this is the default site.
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Gets/sets the optional culture for the site.
    /// </summary>
    public string Culture { get; set; }

    /// <summary>
    /// Gets/sets the available site types.
    /// </summary>
    public IList<ContentTypeModel> SiteTypes { get; set; } = new List<ContentTypeModel>();

    /// <summary>
    /// Gets/sets the available languages.
    /// </summary>
    public IEnumerable<Language> Languages { get; set; } = new List<Language>();

    /// <summary>
    /// Default constructor.
    /// </summary>
    public SiteEditModel()
    {
        UseBlocks = false;
    }
}
