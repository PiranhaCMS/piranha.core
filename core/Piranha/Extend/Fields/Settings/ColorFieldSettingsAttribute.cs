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
/// Base class for field settings.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ColorFieldSettingsAttribute : FieldSettingsAttribute
{
    /// <summary>
    /// Gets/sets if disallowing manual input.
    /// </summary>
    public bool DisallowInput { get; set; }

    /// <summary>
    /// Gets/sets the optional default value for field.
    /// </summary>
    public string DefaultValue { get; set; }
}
