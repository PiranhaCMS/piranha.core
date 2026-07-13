/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Newtonsoft.Json;

namespace Piranha.Data;

[Serializable]
public sealed class PageTranslation
{
    /// <summary>
    /// Gets/sets the page id.
    /// </summary>
    public Guid PageId { get; set; }

    /// <summary>
    /// Gets/sets the language id.
    /// </summary>
    public Guid LanguageId { get; set; }

    /// <summary>
    /// Gets/sets the title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets/sets the navigation title.
    /// </summary>
    public string NavigationTitle { get; set; }

    /// <summary>
    /// Gets/sets the slug.
    /// </summary>
    public string Slug { get; set; }

    /// <summary>
    /// Gets/sets the excerpt.
    /// </summary>
    public string Excerpt { get; set; }

    /// <summary>
    /// Gets/sets the meta title.
    /// </summary>
    public string MetaTitle { get; set; }

    /// <summary>
    /// Gets/sets the meta keywords.
    /// </summary>
    public string MetaKeywords { get; set; }

    /// <summary>
    /// Gets/sets the meta description.
    /// </summary>
    public string MetaDescription { get; set; }

    /// <summary>
    /// Gets/sets the open graph title.
    /// </summary>
    public string OgTitle { get; set; }

    /// <summary>
    /// Gets/sets the open graph description.
    /// </summary>
    public string OgDescription { get; set; }

    /// <summary>
    /// Gets/sets the last modification date.
    /// </summary>
    public DateTime LastModified { get; set; }

    /// <summary>
    /// Gets/sets the page.
    /// </summary>
    [JsonIgnore]
    public Page Page { get; set; }

    /// <summary>
    /// Gets/sets the language.
    /// </summary>
    public Language Language { get; set; }
}
