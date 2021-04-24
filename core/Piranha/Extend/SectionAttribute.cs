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
    /// Attribute for marking a property as a block section. Properties
    /// decorated with this attribute as to return a list of blocks.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SectionAttribute : Attribute
    {
        /// <summary>
        /// Gets/sets the optional title.
        /// </summary>
        public string Title { get; set; }
    }
}
