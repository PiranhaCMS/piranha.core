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
    [FieldType(Name = "Text", Shorthand = "Text", Component = "text-field")]
    public class TextField : SimpleField<string>, ISearchable
    {
        /// <summary>
        /// Implicit operator for converting a string to a field.
        /// </summary>
        /// <param name="str">The string value</param>
        public static implicit operator TextField(string str)
        {
            return new TextField { Value = str };
        }

        /// <summary>
        /// Implicitly converts the Text field to a string.
        /// </summary>
        /// <param name="field">The field</param>
        public static implicit operator string(TextField field)
        {
            return field.Value;
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
