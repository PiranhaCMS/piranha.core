

using Aero.Cms.Extend.Fields;
using Aero.Cms.Extend.Blocks;
using Aero.Cms.Models;

namespace Aero.Cms.AspNetCore.Helpers;

/// <summary>
/// Helper for manipulating media files.
/// </summary>
public class MediaHelper : IMediaHelper
{
    private readonly IApi _api;

    /// <summary>
    /// Default internal constructur.
    /// </summary>
    internal MediaHelper(IApi api)
    {
        _api = api;
    }

    /// <summary>
    /// Resizes the given image to the given dimensions.
    /// </summary>
    /// <param name="image">The image field</param>
    /// <param name="width">The width</param>
    /// <param name="height">The optional height</param>
    /// <returns>The public URL of the resized image</returns>
    public string ResizeImage(ImageField image, int width, int? height = null)
    {
        if (string.IsNullOrEmpty(image)|| image.Id == Guid.Empty.ToString())
            return null;
        return _api.Media.EnsureVersion(image.Id, width, height);
    }

    /// <summary>
    /// Resizes the given image to the given dimensions.
    /// </summary>
    /// <param name="image">The image</param>
    /// <param name="width">The width</param>
    /// <param name="height">The optional width</param>
    /// <returns>The public URL of the resized image</returns>
    public string ResizeImage(Media image, int width, int? height = null)
    {
        if (image is null || image.Id == Guid.Empty.ToString() || image.Type != MediaType.Image)
            return null;
        return _api.Media.EnsureVersion(image.Id, width, height);
    }

    /// <summary>
    /// Resizes the given image block according to the
    /// preferred aspect.
    /// </summary>
    /// <param name="block">The image block</param>
    /// <param name="width">The width</param>
    /// <returns>The public URL of the resized image</returns>
    public string ResizeImage(ImageBlock block, int width)
    {
        if (block == null || block.Body == null || block.Body.Media == null)
            return null;

        int? height = null;

        if (block.Aspect.Value == ImageAspect.Landscape)
        {
            height = Convert.ToInt32(width * 2 / 3);
        }
        else if (block.Aspect.Value == ImageAspect.Portrait)
        {
            height = Convert.ToInt32(width * 3 / 2);
        }
        else if (block.Aspect.Value == ImageAspect.Widescreen)
        {
            height = Convert.ToInt32(width * 9 / 16);
        }
        else if (block.Aspect.Value == ImageAspect.Square)
        {
            height = width;
        }
        return _api.Media.EnsureVersion(block.Body.Media.Id, width, height);
    }
}
