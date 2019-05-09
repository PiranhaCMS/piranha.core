/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Collections.Generic;

namespace Piranha.Manager.Models
{
    public class MediaListModel
    {
        public class FolderItem
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public int ItemCount { get; set; }
        }

        public class MediaItem
        {
            public Guid Id { get; set; }
            public Guid? FolderId { get; set; }
            public string Type { get; set; }
            public string Filename { get; set; }
            public string ContentType { get; set; }
            public string PublicUrl { get; set; }
            public string Size { get; set; }
            public int? Width { get; set; }
            public int? Height { get; set; }
            public string LastModified { get; set; }
        }

        /// <summary>
        /// Gets/sets the available folders.
        /// </summary>
        public IList<FolderItem> Folders { get; set; } = new List<FolderItem>();

        /// <summary>
        /// Gets/sets the available media items.
        /// </summary>
        public IList<MediaItem> Media { get; set; } = new List<MediaItem>();

        /// <summary>
        /// Gets/sets the optional folder id.
        /// </summary>
        public Guid? CurrentFolderId { get; set; }

        /// <summary>
        /// Gets/sets the optional parent id.
        /// </summary>
        public Guid? ParentFolderId { get; set; }

        /// <summary>
        /// Gets/sets the optional status message from the last operation.
        /// </summary>
        public StatusMessage Status { get; set; }
    }
}