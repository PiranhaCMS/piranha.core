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
    public class Media : MediaItem
    {
        /// <summary>
        /// Gets/sets the optional folder id.
        /// </summary>
        public Guid? FolderId { get; set; }

        /// <summary>
        /// Gets/sets the optional file size.
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Gets/sets the optional content type.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets/sets the optional description.
        /// </summary>
        public string Description { get; set; }
    }
}
