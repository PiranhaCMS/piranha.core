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
    [FieldType(Name = "Readonly", Shorthand = "Readonly", Component = "readonly-field")]
    public class ReadonlyField : SimpleField<string>
    {
        /// <summary>
        /// Implicit operator for converting a string to a field.
        /// </summary>
        /// <param name="str">The string value</param>
        public static implicit operator ReadonlyField(string str)
        {
            return new ReadonlyField { Value = str };
        }

        /// <summary>
        /// Implicitly converts the readonly field to a string.
        /// </summary>
        /// <param name="field">The field</param>
        public static implicit operator string(ReadonlyField field)
        {
            return field.Value;
        }
    }
}
