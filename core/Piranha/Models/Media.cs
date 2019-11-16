/*
 * Copyright (c) 2017-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.Collections.Generic;

namespace Piranha.Models
{
    [Serializable]
    public sealed class Media : MediaBase
    {
        /// <summary>
        /// Gets/sets the available versions.
        /// </summary>
        public IList<MediaVersion> Versions { get; set; } = new List<MediaVersion>();
    }

    [Serializable]
    public abstract class MediaBase
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the optional folder id.
        /// </summary>
        public Guid? FolderId { get; set; }

        /// <summary>
        /// Gets/sets the media type.
        /// </summary>
        public MediaType Type { get; set; }

        /// <summary>
        /// Gets/sets the filename.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Gets/sets the content type.
        /// </summary>
        /// <returns></returns>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets/sets the file size in bytes.
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Gets/sets the public url.
        /// </summary>
        public string PublicUrl { get; set; }

        /// <summary>
        /// Gets/sets the optional width. This only applies
        /// if the media asset is an image.
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// Gets/sets the optional height. This only applies
        /// if the media asset is an image.
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// Gets/sets the created date.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets/sets the last modification date.
        /// </summary>
        public DateTime LastModified { get; set; }
    }
}