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
    [FieldType(Name = "Checkbox", Shorthand = "Checkbox", Component = "checkbox-field")]
    public class CheckBoxField : SimpleField<bool>
    {
        /// <summary>
        /// Implicit operator for converting a bool to a field.
        /// </summary>
        /// <param name="str">The string value</param>
        public static implicit operator CheckBoxField(bool str)
        {
            return new CheckBoxField { Value = str };
        }

        /// <summary>
        /// Implicitly converts the CheckBox field to a bool.
        /// </summary>
        /// <param name="field">The field</param>
        public static implicit operator bool(CheckBoxField field)
        {
            return field.Value;
        }
    }
}
