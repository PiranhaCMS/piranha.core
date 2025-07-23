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
/// Base class for simple single type fields.
/// </summary>
/// <typeparam name="T">The field type</typeparam>
public abstract class SimpleField<T> : Field, IEquatable<SimpleField<T>>
{
    /// <summary>
    /// Gets/sets the field value.
    /// </summary>
    public T Value { get; set; }

    /// <inheritdoc />
    public override string GetTitle()
    {
        if (Value == null)
        {
            return null;
        }

        var title = Value.ToString();

        if (title.Length > 40)
        {
            title = title.Substring(0, 40) + "...";
        }
        return title;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Value.Equals(default(T)) ? 0 : Value.GetHashCode();
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        if (obj is SimpleField<T> field)
        {
            return Equals(field);
        }
        return false;
    }

    /// <summary>
    /// Checks if the given field is equal to the field.
    /// </summary>
    /// <param name="obj">The field</param>
    /// <returns>True if the fields are equal</returns>
    public virtual bool Equals(SimpleField<T> obj)
    {
        return obj != null && EqualityComparer<T>.Default.Equals(Value, obj.Value);
    }

    /// <summary>
    /// Checks if the fields are equal.
    /// </summary>
    /// <param name="field1">The first field</param>
    /// <param name="field2">The second field</param>
    /// <returns>True if the fields are equal</returns>
    public static bool operator ==(SimpleField<T> field1, SimpleField<T> field2)
    {
        if ((object)field1 != null && (object)field2 != null)
        {
            return field1.Equals(field2);
        }

        if ((object)field1 == null && (object)field2 == null)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if the fields are not equal.
    /// </summary>
    /// <param name="field1">The first field</param>
    /// <param name="field2">The second field</param>
    /// <returns>True if the fields are equal</returns>
    public static bool operator !=(SimpleField<T> field1, SimpleField<T> field2)
    {
        return !(field1 == field2);
    }
}
