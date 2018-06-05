/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System;

namespace Piranha.Data
{
    public sealed class MediaVersion : IModel 
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the id of the media this is 
        /// a version of.
        /// </summary>
        public Guid MediaId { get; set; }

        /// <summary>
        /// Gets/sets the file size in bytes.
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Gets/sets the width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets/sets the optional height.
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// Gets/sets the file extension of the generated version.
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// Gets/sets the media this is a version of.
        /// </summary>
        public Media Media { get; set; }
    }
}