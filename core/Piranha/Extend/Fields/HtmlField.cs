/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Text.RegularExpressions;

namespace Piranha.Extend.Fields
{
    [FieldType(Name = "Html", Shorthand = "Html", Component = "html-field")]
    public class HtmlField : SimpleField<string>, ISearchable
    {
        /// <summary>
        /// Implicit operator for converting a string to a field.
        /// </summary>
        /// <param name="str">The string value</param>
        public static implicit operator HtmlField(string str)
        {
            return new HtmlField { Value = str };
        }

        /// <summary>
        /// Implicitly converts the Html field to a string.
        /// </summary>
        /// <param name="field">The field</param>
        public static implicit operator string(HtmlField field)
        {
            return field.Value;
        }

        /// <summary>
        /// Gets the list item title if this field is used in
        /// a collection regions.
        /// </summary>
        public override string GetTitle()
        {
            if (Value != null)
            {
                var title = Regex.Replace(Value, @"<[^>]*>", "");

                if (title.Length > 40)
                {
                    title = title.Substring(0, 40) + "...";
                }
                return title;
            }
            return null;
        }

        /// <summary>
        /// Gets the content that should be indexed for searching.
        /// </summary>
        public string GetIndexedContent()
        {
            return !string.IsNullOrEmpty(Value) ? Value : "";
        }
    }
}
