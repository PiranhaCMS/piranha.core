/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Extend.Fields.Settings
{
    /// <summary>
    /// Base class for required settings.
    /// </summary>
    public abstract class RequiredFieldSettingsAttribute : FieldSettingsAttribute
    {
        /// <summary>
        /// Gets/sets if the string field is required. The default
        /// value is false.
        /// </summary>
        public bool IsRequired { get; set; }
    }
}
