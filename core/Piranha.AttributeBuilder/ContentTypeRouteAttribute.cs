/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.AttributeBuilder;

/// <summary>
/// Attribute for marking a class as a content type.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ContentTypeRouteAttribute : Attribute
{
    /// <summary>
    /// Gets/sets the display title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets/sets the internal route.
    /// </summary>
    public string Route { get; set; }
}
