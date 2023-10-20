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
/// Field for a datetime value.
/// </summary>
[FieldType(Name = "Date", Shorthand = "Date", Component = "date-field")]
public class DateField : SimpleField<DateTime?>
{
    /// <summary>
    /// Converts the given datetime to a field.
    /// </summary>
    /// <param name="date">The datetime</param>
    public static implicit operator DateField(DateTime date)
    {
        return new DateField { Value = date };
    }
}
