/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Extend.Fields;

namespace Piranha.Extend.Serializers;

/// <summary>
/// Serializer for document fields.
/// </summary>
public class DocumentFieldSerializer : ISerializer
{
    /// <inheritdoc />
    public string Serialize(object obj)
    {
        if (obj is DocumentField field)
        {
            return field.Id.ToString();
        }
        throw new ArgumentException("The given object doesn't match the serialization type");
    }

    /// <inheritdoc />
    public object Deserialize(string str)
    {
        return new DocumentField
        {
            Id = !string.IsNullOrEmpty(str) ? new Guid(str) : (Guid?)null
        };
    }
}
