/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Extend.Serializers;

/// <summary>
/// Serialized for checkbox fields.
/// </summary>
/// <typeparam name="T">The field type</typeparam>
public class CheckBoxFieldSerializer<T> : ISerializer where T : Fields.SimpleField<bool>
{
    /// <inheritdoc />
    public string Serialize(object obj)
    {
        if (obj is T field)
        {
            return field.Value.ToString();
        }
        throw new ArgumentException("The given object doesn't match the serialization type");
    }

    /// <inheritdoc />
    public object Deserialize(string str)
    {
        var ret = Activator.CreateInstance<T>();
        ret.Value = bool.Parse(str);

        return ret;
    }
}
