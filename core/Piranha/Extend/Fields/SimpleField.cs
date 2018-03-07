﻿/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha.Extend.Fields
{
    /// <summary>
    /// Base class for simple single type fields.
    /// </summary>
    /// <typeparam name="T">The field type</typeparam>
    public abstract class SimpleField<T> : Field
    {
        /// <summary>
        /// Gets/sets the field value.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Gets the list item title if this field is used in
        /// a collection regions.
        /// </summary>
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
    }
}
