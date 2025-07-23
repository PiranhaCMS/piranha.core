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
/// Serializer for string based fields.
/// </summary>
/// <typeparam name="T">The field type</typeparam>
public class StringFieldSerializer<T> : ISerializer where T : Fields.SimpleField<string>
{
    /// <inheritdoc />
    public string Serialize(object obj)
    {
        if (obj is T field)
        {
            return field.Value;
        }
        throw new ArgumentException("The given object doesn't match the serialization type");
    }

    /// <inheritdoc />
    public object Deserialize(string str)
    {
        var ret = Activator.CreateInstance<T>();
        ret.Value = str;

        return ret;
    }
}
