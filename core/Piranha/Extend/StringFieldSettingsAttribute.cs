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
    /// Base class for field settings.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class StringFieldSettingsAttribute : FieldSettingsAttribute
    {
        /// <summary>
        /// Gets/sets the optional max length of the field.
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// Gets/sets the optional default value for field.
        /// </summary>
        public string DefaultValue { get; set; }
    }
}
