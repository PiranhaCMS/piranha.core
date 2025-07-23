/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Models;

namespace Piranha.Runtime;

public class MediaManager
{
    /// <summary>
    /// A registered media type.
    /// </summary>
    public class MediaTypeItem
    {
        /// <summary>
        /// Gets/sets the file extension.
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// Gets/sets the content type.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// If image processing should be applied to this media type.
        /// </summary>
        public bool AllowProcessing { get; set; }
    }

    /// <summary>
    /// A list of media types.
    /// </summary>
    public class MediaTypeList : List<MediaTypeItem>
    {
        private readonly bool _allowProcessing;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="allowProcessing">If image processing should be applied by default</param>
        public MediaTypeList(bool allowProcessing = false)
        {
            _allowProcessing = allowProcessing;
        }

        /// <summary>
        /// Adds a new media type.
        /// </summary>
        /// <param name="extension">The file extension</param>
        /// <param name="contentType">The content type</param>
        /// <param name="allowProcessing">If image processing should be allowed</param>
        public void Add(string extension, string contentType, bool? allowProcessing = null)
        {
            Add(new MediaTypeItem
            {
                Extension = extension.ToLower(),
                ContentType = contentType,
                AllowProcessing = allowProcessing.HasValue ? allowProcessing.Value : _allowProcessing
            });
        }

        /// <summary>
        /// Gets if the list contains an item with the given extension.
        /// </summary>
        /// <param name="extension">The file extension</param>
        /// <returns>If the extension exists</returns>
        public bool ContainsExtension(string extension)
        {
            return this.Any(t => t.Extension.Equals(extension, System.StringComparison.OrdinalIgnoreCase));
        }
    }

    /// <summary>
    /// Gets/sets the currently accepted document extensions.
    /// </summary>
    public MediaTypeList Documents { get; set; } = new();

    /// <summary>
    /// Gets/sets the currently accepted image extensions.
    /// </summary>
    public MediaTypeList Images { get; set; } = new(true);

    /// <summary>
    /// Gets/sets the currently accepted video extensions.
    /// </summary>
    public MediaTypeList Videos { get; set; } = new();

    /// <summary>
    /// Gets/sets the currently accepted audio extensions.
    /// </summary>
    public MediaTypeList Audio { get; set; } = new();

    /// <summary>
    /// Gets/sets the currently accepted resource extensions.
    /// </summary>
    public MediaTypeList Resources { get; set; } = new();

    /// <summary>
    /// Gets/sets the currently registered meta properties for media.
    /// </summary>
    public IList<string> MetaProperties { get; set; } = new List<string>();

    /// <summary>
    /// Checks if the given filename has a supported extension.
    /// </summary>
    /// <param name="filename">The path or filename</param>
    /// <returns>If it is supported</returns>
    public bool IsSupported(string filename)
    {
        var extension = Path.GetExtension(filename);

        return Documents.ContainsExtension(extension) ||
            Images.ContainsExtension(extension) ||
            Videos.ContainsExtension(extension) ||
            Audio.ContainsExtension(extension) ||
            Resources.ContainsExtension(extension);
    }

    /// <summary>
    /// Gets the media type for the given filename based on
    /// its extension.
    /// </summary>
    /// <param name="filename">The path or filename</param>
    /// <returns>The media type</returns>
    public MediaType GetMediaType(string filename)
    {
        var extension = Path.GetExtension(filename);

        if (Documents.ContainsExtension(extension))
        {
            return MediaType.Document;
        }

        if (Images.ContainsExtension(extension))
        {
            return MediaType.Image;
        }
        if (Videos.ContainsExtension(extension))
        {
            return MediaType.Video;
        }
        if (Audio.ContainsExtension(extension))
        {
            return MediaType.Audio;
        }
        if (Resources.ContainsExtension(extension))
        {
            return MediaType.Resource;
        }

        return MediaType.Unknown;
    }

    /// <summary>
    /// Gets the content type for the given filename based on
    /// its extension.
    /// </summary>
    /// <param name="filename">The path or filename</param>
    /// <returns>The media type</returns>
    public string GetContentType(string filename)
    {
        var extension = Path.GetExtension(filename).ToLower();
        MediaTypeItem item = null;

        if ((item = Documents.SingleOrDefault(t => t.Extension == extension)) != null)
        {
            return item.ContentType;
        }

        if ((item = Images.SingleOrDefault(t => t.Extension == extension)) != null)
        {
            return item.ContentType;
        }
        if ((item = Videos.SingleOrDefault(t => t.Extension == extension)) != null)
        {
            return item.ContentType;
        }
        if ((item = Audio.SingleOrDefault(t => t.Extension == extension)) != null)
        {
            return item.ContentType;
        }
        if ((item = Resources.SingleOrDefault(t => t.Extension == extension)) != null)
        {
            return item.ContentType;
        }

        return "application/octet-stream";
    }

    /// <summary>
    /// Gets the media type item for the given filename based on its extensions.
    /// </summary>
    /// <param name="filename">The path or filename</param>
    /// <returns>The media type item</returns>
    public MediaTypeItem GetItem(string filename)
    {
        var extension = Path.GetExtension(filename).ToLower();
        MediaTypeItem item = null;

        if ((item = Documents.SingleOrDefault(t => t.Extension == extension)) != null)
        {
            return item;
        }

        if ((item = Images.SingleOrDefault(t => t.Extension == extension)) != null)
        {
            return item;
        }
        if ((item = Videos.SingleOrDefault(t => t.Extension == extension)) != null)
        {
            return item;
        }
        if ((item = Audio.SingleOrDefault(t => t.Extension == extension)) != null)
        {
            return item;
        }
        if ((item = Resources.SingleOrDefault(t => t.Extension == extension)) != null)
        {
            return item;
        }

        return null;
    }
}
