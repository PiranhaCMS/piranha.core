/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;

namespace Piranha.Extend.Fields
{
    [FieldType(Name = "Date", Shorthand = "Date", Component = "date-field")]
    public class DateField : SimpleField<DateTime?>
    {
        public static implicit operator DateField(DateTime date)
        {
            return new DateField { Value = date };
        }
    }
}
