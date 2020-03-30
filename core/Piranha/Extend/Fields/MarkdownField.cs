/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Extend.Fields
{
    [FieldType(Name = "Markdown", Shorthand = "Markdown", Component = "markdown-field")]
    public class MarkdownField : SimpleField<string>, ISearchable
    {
        /// <summary>
        /// Implicit operator for converting a string to a field.
        /// </summary>
        /// <param name="str">The string value</param>
        public static implicit operator MarkdownField(string str)
        {
            return new MarkdownField { Value = str };
        }

        /// <summary>
        /// Implicitly converts the markdown field to a HTML string.
        /// </summary>
        /// <param name="field">The field</param>
        public static implicit operator string(MarkdownField field)
        {
            return field.ToHtml();
        }

        /// <summary>
        /// Transforms the markdown field to HTML.
        /// </summary>
        /// <returns>The HTML string</returns>
        public string ToHtml()
        {
            return App.Markdown.Transform(Value);
        }

        /// <summary>
        /// Gets the content that should be indexed for searching.
        /// </summary>
        public string GetIndexedContent()
        {
            return !string.IsNullOrEmpty(Value) ? ToHtml() : "";
        }
    }
}
