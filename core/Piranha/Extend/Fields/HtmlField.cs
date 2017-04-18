/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha.Extend.Fields
{
    [Field(Name = "Html", Shorthand = "Html")]
    public class HtmlField : SimpleField<string>
    {
        /// <summary>
        /// Implicit operator for converting a string to a field.
        /// </summary>
        /// <param name="str">The string value</param>
        public static implicit operator HtmlField(string str) {
            return new HtmlField() { Value = str };
        }

        /// <summary>
        /// Implicitly converts the Html field to a string.
        /// </summary>
        /// <param name="field">The field</param>
        public static implicit operator string(HtmlField field) {
            return field.Value;
        }
    }
}
