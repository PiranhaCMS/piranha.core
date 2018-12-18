/*
 * Copyright (c) 2016-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Piranha.Models;

namespace Piranha.Runtime
{
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
        }

        /// <summary>
        /// A list of media types.
        /// </summary>
        public class MediaTypeList : List<MediaTypeItem>
        {
            /// <summary>
            /// Adds a new media type.
            /// </summary>
            /// <param name="extension">The file extension</param>
            /// <param name="contentType">The content type</param>
            public void Add(string extension, string contentType)
            {
                Add(new MediaTypeItem
                {
                    Extension = extension,
                    ContentType = contentType
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
        public MediaTypeList Documents { get; set; } = new MediaTypeList();

        /// <summary>
        /// Gets/sets the currently accepted image extensions.
        /// </summary>
        public MediaTypeList Images { get; set; } = new MediaTypeList();

        /// <summary>
        /// Gets/sets the currently accepted video extensions.
        /// </summary>
        public MediaTypeList Videos { get; set; } = new MediaTypeList();

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
                Videos.ContainsExtension(extension);
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
            else if (Images.ContainsExtension(extension))
            {
                return MediaType.Image;
            }
            else if (Videos.ContainsExtension(extension))
            {
                return MediaType.Video;
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
            var extension = Path.GetExtension(filename);
            MediaTypeItem item = null;

            if ((item = Documents.SingleOrDefault(t => t.Extension == extension)) != null)
            {
                return item.ContentType;
            }
            else if ((item = Images.SingleOrDefault(t => t.Extension == extension)) != null)
            {
                return item.ContentType;
            }
            else if ((item = Videos.SingleOrDefault(t => t.Extension == extension)) != null)
            {
                return item.ContentType;
            }
            return "application/octet-stream";
        }
    }
}
