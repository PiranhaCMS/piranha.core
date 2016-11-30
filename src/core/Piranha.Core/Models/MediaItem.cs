/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;

namespace Piranha.Models
{
    public class MediaItem
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the media type. 
        /// </summary>
        public MediaType Type { get; set; }

        /// <summary>
        /// Gets/sets the optional filename.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets/sets the public url.
        /// </summary>
        public string PublicUrl { get; set; }

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
