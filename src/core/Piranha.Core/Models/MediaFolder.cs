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
    public class MediaFolder
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the optional parent id if this is a sub folder.
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Gets/sets the folder title.
        /// </summary>
        public string Title { get; set; }
    }
}
