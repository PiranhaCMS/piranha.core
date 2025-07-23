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
/// Settings for content fields.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ContentFieldSettingsAttribute : FieldSettingsAttribute
{
    /// <summary>
    /// Gets/sets the currently allowed group.
    /// </summary>
    public string Group { get; set; }
}
