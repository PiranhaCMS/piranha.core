/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Extend.Fields;

namespace Piranha.Models;

/// <summary>
/// Base class for generic content.
/// </summary>
public abstract class GenericContent : ContentBase
{
    /// <summary>
    /// Gets/sets the optional primary image.
    /// </summary>
    public ImageField PrimaryImage { get; set; } = new ImageField();

    /// <summary>
    /// Gets/sets the optional excerpt.
    /// </summary>
    public string Excerpt { get; set; }
}
