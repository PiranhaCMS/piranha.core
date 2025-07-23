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
using Piranha.Extend.Blocks;
using Piranha.Models;

namespace Piranha.AspNetCore.Helpers;

/// <summary>
/// Helper for manipulating media files.
/// </summary>
public interface IMediaHelper
{
    /// <summary>
    /// Resizes the given image to the given dimensions.
    /// </summary>
    /// <param name="image">The image field</param>
    /// <param name="width">The width</param>
    /// <param name="height">The optional height</param>
    /// <returns>The public URL of the resized image</returns>
    string ResizeImage(ImageField image, int width, int? height = null);

    /// <summary>
    /// Resizes the given image to the given dimensions.
    /// </summary>
    /// <param name="image">The image</param>
    /// <param name="width">The width</param>
    /// <param name="height">The optional width</param>
    /// <returns>The public URL of the resized image</returns>
    string ResizeImage(Media image, int width, int? height = null);

    /// <summary>
    /// Resizes the given image block according to the
    /// preferred aspect.
    /// </summary>
    /// <param name="block">The image block</param>
    /// <param name="width">The width</param>
    /// <returns>The public URL of the resized image</returns>
    string ResizeImage(ImageBlock block, int width);
}
