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

namespace Piranha.Extend
{
    /// <summary>
    /// Attribute for marking a property as a region.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RegionDescriptionAttribute : Attribute
    {
        /// <summary>
        /// Gets/sets the optional description text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RegionDescriptionAttribute() { }

        /// <summary>
        /// Creates a new description attribute and
        /// sets the description text.
        /// </summary>
        /// <param name="text">The description text</param>
        public RegionDescriptionAttribute(string text)
        {
            Text = text;
        }
    }
}