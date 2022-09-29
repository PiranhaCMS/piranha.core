/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Extend.Blocks;

/// <summary>
/// The different image aspects available
/// </summary>
public enum ImageAspect
{
    /// <summary>
    /// Keeps the original image aspect
    /// </summary>
    Original,
    /// <summary>
    /// Crops the image to landscape (3x2)
    /// </summary>
    Landscape,
    /// <summary>
    /// Crops the image to portrait (2x3)
    /// </summary>
    Portrait,
    /// <summary>
    /// Crops the image to widescreen (16x9)
    /// </summary>
    Widescreen,
    /// <summary>
    /// Crops the image to square (1x1)
    /// </summary>
    Square
}
