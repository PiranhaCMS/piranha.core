/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;

namespace Piranha.Areas.Manager.Models
{
    /// <summary>
    /// Model for creating a new media folder.
    /// </summary>
    public class MediaFolderModel
    {
        /// <summary>
        /// Gets/sets the parent id.
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Gets/sets the folder name.
        /// </summary>
        public string Name { get; set; }
    }
}