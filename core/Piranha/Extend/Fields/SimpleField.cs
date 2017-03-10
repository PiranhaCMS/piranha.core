/*
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
    }
}
