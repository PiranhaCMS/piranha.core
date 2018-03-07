/*
 * Copyright (c) 2017 Håkan Edling
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
    [Field(Name = "Date", Shorthand = "Date")]
    public class DateField : SimpleField<DateTime?>
    {
        public static implicit operator DateField(DateTime date)
        {
            return new DateField { Value = date };
        }
    }
}
