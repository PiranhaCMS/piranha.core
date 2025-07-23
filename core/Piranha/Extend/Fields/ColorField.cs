/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Extend.Fields;

/// <summary>
/// Field for a color value.
/// </summary>
[FieldType(Name = "Color", Shorthand = "Color", Component = "color-field")]
public class ColorField : SimpleField<string>
{
    /// <summary>
    /// Implicit operator for converting a string to a field.
    /// </summary>
    /// <param name="str">The color value</param>
    public static implicit operator ColorField(string str)
    {
        return new ColorField { Value = str };
    }

    /// <summary>
    /// Implicitly converts the color field to a string.
    /// </summary>
    /// <param name="field">The field</param>
    public static implicit operator string(ColorField field)
    {
        return field.Value;
    }
}
