/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.IO;

namespace Piranha.Models
{
    /// <summary>
    /// Abstract class for media content.
    /// </summary>
    public abstract class MediaContent
    {
        /// <summary>
        /// Gets/sets the optional id. If this is empty a new media
        /// entry is created, otherwise the specified media
        /// is updated.
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Gets/sets the optional folder id this media should be placed in.
        /// </summary>
        public Guid? FolderId { get; set; }

        /// <summary>
        /// Gets/sets the filename.
        /// </summary>
        public string Filename { get; set; }
    }

    /// <summary>
    /// Binary media content.
    /// </summary>
    public class BinaryMediaContent : MediaContent
    {
        /// <summary>
        /// Gets/sets the byte data of the media.
        /// </summary>
        public byte[] Data { get; set; }
    }

    /// <summary>
    /// Stream media content.
    /// </summary>
    public class StreamMediaContent : MediaContent
    {
        /// <summary>
        /// Gets/sets the stream the contains the media content.
        /// </summary>
        public Stream Data { get; set; }
    }
}
