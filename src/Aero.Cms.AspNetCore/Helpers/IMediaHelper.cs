

using Aero.Cms.Extend.Fields;
using Aero.Cms.Extend.Blocks;
using Aero.Cms.Models;

namespace Aero.Cms.AspNetCore.Helpers;

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
