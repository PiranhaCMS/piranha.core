/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Piranha.Models;
using System;

namespace Piranha.AttributeBuilder
{
    /// <summary>
    /// Attribute for marking a property as a field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldAttribute : Attribute
    {
        /// <summary>
        /// Gets/sets the optional title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the field options.
        /// </summary>
        public FieldOption Options { get; set; }

        /// <summary>
        /// Gets/sets the optional placeholder for
        /// text based fields.
        /// </summary>
        public string Placeholder { get; set; }
    }
}
