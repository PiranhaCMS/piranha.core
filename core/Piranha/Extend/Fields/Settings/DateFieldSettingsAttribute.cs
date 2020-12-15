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

namespace Piranha.Extend.Fields.Settings
{
    /// <summary>
    /// Settings for date fields.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DateFieldSettingsAttribute : RequiredFieldSettingsAttribute
    {
        /// <summary>
        /// Gets/sets the optional default value for field.
        /// </summary>
        public string DefaultValue { get; set; }
    }
}
