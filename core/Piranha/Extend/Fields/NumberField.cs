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
    [FieldType(Name = "Number", Shorthand = "Number", Component = "number-field")]
    public class NumberField : SimpleField<int?>
    {
        /// <summary>
        /// Implicit operator for converting a int to a field.
        /// </summary>
        /// <param name="number">The integer value</param>
        public static implicit operator NumberField(int number)
        {
            return new NumberField { Value = number };
        }

        /// <summary>
        /// Implicitly converts the Number field to a int.
        /// </summary>
        /// <param name="field">The field</param>
        public static implicit operator int? (NumberField field)
        {
            return field.Value;
        }
    }
}
