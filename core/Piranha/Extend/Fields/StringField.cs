﻿/*
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
    [Field(Name = "String", Shorthand = "String")]
    public class StringField : SimpleField<string>
    {
        /// <summary>
        /// Implicit operator for converting a string to a field.
        /// </summary>
        /// <param name="str">The string value</param>
        public static implicit operator StringField(string str)
        {
            return new StringField { Value = str };
        }

        /// <summary>
        /// Implicitly converts the String field to a string.
        /// </summary>
        /// <param name="field">The field</param>
        public static implicit operator string(StringField field)
        {
            return field.Value;
        }
    }
}
