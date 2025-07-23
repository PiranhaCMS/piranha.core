/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Extend.Fields.Settings;

/// <summary>
/// Settings for string fields.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class StringFieldSettingsAttribute : FieldSettingsAttribute
{
    /// <summary>
    /// Gets/sets the optional default value for field.
    /// </summary>
    public string DefaultValue { get; set; }

    /// <summary>
    /// Gets/sets the optional max length of the field. A value
    /// of 0 means that the field has no max length.
    /// </summary>
    public int MaxLength { get; set; }
}
