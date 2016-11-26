/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.AspNetCore.Html;

/// <summary>
/// Extension methods for simplifying field usage.
/// </summary>
public static class FieldExtensionscs
{
    /// <summary>
    /// Gets a HTML string from the current field.
    /// </summary>
    /// <param name="field">The current html field</param>
    /// <returns>The HTML string</returns>
    public static HtmlString Html(this Piranha.Extend.Fields.HtmlField field) {
        return new HtmlString(field.Value);
    }

    /// <summary>
    /// Gets a html string from the current field.
    /// </summary>
    /// <param name="field">The current markdown field</param>
    /// <returns>The HTML string</returns>
    public static HtmlString Html(this Piranha.Extend.Fields.MarkdownField field) {
        return new HtmlString(field.ToHtml());
    }
}
