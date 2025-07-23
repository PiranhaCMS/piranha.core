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
/// Base class for content type editor attributes.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ContentTypeEditorAttribute : Attribute
{
    /// <summary>
    /// Gets/sets the editor component.
    /// </summary>
    public string Component { get; set; }

    /// <summary>
    /// Gets/sets the optional icon css.
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// Gets/sets the title.
    /// </summary>
    public string Title { get; set; }
}
