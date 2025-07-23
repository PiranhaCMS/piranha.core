/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Data;

[Serializable]
public sealed class Site : ContentBase<SiteField>
{
    /// <summary>
    /// Gets/sets the language id.
    /// </summary>
    public Guid? LanguageId { get; set; }

    /// <summary>
    /// Gets/sets the optional site type id.
    /// </summary>
    public string SiteTypeId { get; set; }

    /// <summary>
    /// Gets/sets the internal textual id.
    /// </summary>
    public string InternalId { get; set; }

    /// <summary>
    /// Gets/sets the optional description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets/sets the optional logo image id.
    /// </summary>
    public Guid? LogoId { get; set; }

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
    /// Gets/sets the global last modification date
    /// of the site's content.
    /// </summary>
    public DateTime? ContentLastModified { get; set; }

    /// <summary>
    /// Gets/sets the selected language.
    /// </summary>
    public Language Language { get; set; }
}
