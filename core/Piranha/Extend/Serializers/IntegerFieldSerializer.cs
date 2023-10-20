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
/// Serializer for integer based fields.
/// </summary>
/// <typeparam name="T">The fields type</typeparam>
public class IntegerFieldSerializer<T> : ISerializer where T : Fields.SimpleField<int?>
{
    /// <inheritdoc />
    public string Serialize(object obj)
    {
        if (obj is T field)
        {
            if (field.Value.HasValue)
            {
                return field.Value.Value.ToString();
            }
            return null;
        }
        throw new ArgumentException("The given object doesn't match the serialization type");
    }

    /// <inheritdoc />
    public object Deserialize(string str)
    {
        var field = Activator.CreateInstance<T>();

        if (!string.IsNullOrWhiteSpace(str))
        {
            try
            {
                field.Value = int.Parse(str);
            }
            catch
            {
                // Let's not throw an exception, let's just
                // return a new empty field.
                field.Value = null;
            }
        }
        return field;
    }
}
