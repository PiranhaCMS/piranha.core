using System;
using System.Collections.Generic;
using System.Text;

namespace Piranha.Extend.Fields
{
    [Field(Name = "Checkbox", Shorthand = "Checkbox")]
    public class CheckBoxField : SimpleField<bool>
    {
        /// <summary>
        /// Implicit operator for converting a bool to a field.
        /// </summary>
        /// <param name="str">The string value</param>
        public static implicit operator CheckBoxField(bool str)
        {
            return new CheckBoxField() { Value = str };
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
