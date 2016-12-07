/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Collections.Generic;
using System.IO;
using Piranha.Models;

namespace Piranha
{
    public class MediaTypes
    {
        #region Properties
        /// <summary>
        /// Gets/sets the currently accepted document extensions.
        /// </summary>
        public IList<string> Documents { get; set; }

        /// <summary>
        /// Gets/sets the currently accepted image extensions.
        /// </summary>
        public IList<string> Images { get; set; }

        /// <summary>
        /// Gets/sets the currently accepted video extensions.
        /// </summary>
        public IList<string> Videos { get; set; }
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MediaTypes() {
            Documents = new List<string>();
            Images = new List<string>();
            Videos = new List<string>();
        }

        /// <summary>
        /// Checks if the given filename has a supported extension.
        /// </summary>
        /// <param name="filename">The path or filename</param>
        /// <returns>If it is supported</returns>
        public bool IsSupported(string filename) {
            var extension = Path.GetExtension(filename);
            return Documents.Contains(extension) || Images.Contains(extension) || Videos.Contains(extension);
        }

        /// <summary>
        /// Gets the media type for the given filename based on
        /// its extension.
        /// </summary>
        /// <param name="filename">The path or filename</param>
        /// <returns>The media type</returns>
        public MediaType GetMediaType(string filename) {
            var extension = Path.GetExtension(filename);

            if (Documents.Contains(extension))
                return MediaType.Document;
            else if (Images.Contains(extension))
                return MediaType.Image;
            else if (Videos.Contains(extension))
                return MediaType.Video;
            return MediaType.Unknown;
        }
    }
}
