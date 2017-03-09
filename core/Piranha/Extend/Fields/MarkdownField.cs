/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha.Extend.Fields
{
    [Field(Name = "Markdown", Shorthand = "Markdown")]
    public class MarkdownField : SimpleField<string>
    {
        /// <summary>
        /// Implicit operator for converting a string to 
        /// markdown field.
        /// </summary>
        /// <param name="str">The markdown string value</param>
        public static implicit operator MarkdownField(string str) {
            return new MarkdownField() { Value = str };
        }

        /// <summary>
        /// Transforms the markdown field to HTML.
        /// </summary>
        /// <returns>The HTML string</returns>
        public string ToHtml() {
            return App.Markdown.Transform(Value);
        }
    }
}
