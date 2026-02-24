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
public sealed class ContentTranslation
{
    /// <summary>
    /// Gets/sets the content id.
    /// </summary>
    public string ContentId { get; set; }

    /// <summary>
    /// Gets/sets the language id.
    /// </summary>
    public string LanguageId { get; set; }

    /// <summary>
    /// Gets/sets the main title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets/sets the optional excerpt.
    /// </summary>
    public string Excerpt { get; set; }

    /// <summary>
    /// Gets/sets the last modification date.
    /// </summary>
    public DateTime LastModified { get; set; }

    /// <summary>
    /// Gets/sets the content.
    /// </summary>
    public Content Content { get; set; }

    /// <summary>
    /// Gets/sets the language.
    /// </summary>
    public Language Language { get; set; }
}
