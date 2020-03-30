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
using System.Collections.Generic;

namespace Piranha.Manager.Models
{
    /// <summary>
    /// List model for the media view.
    /// </summary>
    public class MediaListModel
    {
        public static string ListView { get; } = "list";
        public static string GalleryView { get; } = "gallery";

        /// <summary>
        /// A folder item in the list view.
        /// </summary>
        public class FolderItem
        {
            /// <summary>
            /// Gets/sets the unique id.
            /// </summary>
            public Guid Id { get; set; }

            /// <summary>
            /// Gets/sets the name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets/sets the item count in the folder.
            /// </summary>
            public int ItemCount { get; set; }
        }

        /// <summary>
        /// A media item in the list view.
        /// </summary>
        public class MediaItem
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
            public string Type { get; set; }

            /// <summary>
            /// Gets/sets the filename of the uploaded media.
            /// </summary>
            public string Filename { get; set; }

            /// <summary>
            /// Gets/sets the content type of the uploaded media.
            /// </summary>
            public string ContentType { get; set; }

            /// <summary>
            /// Gets/sets the optional title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets/sets the optional alt text.
            /// </summary>
            public string AltText { get; set; }

            /// <summary>
            /// Gets/sets the optional description.
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Gets/sets the optional properties.
            /// </summary>
            public IList<KeyValuePair<string, string>> Properties { get; set; } = new List<KeyValuePair<string, string>>();

            /// <summary>
            /// Gets/sets the public url used to access the uploaded media.
            /// </summary>
            public string PublicUrl { get; set; }

            /// <summary>
            /// Gets/sets the file size.
            /// </summary>
            public string Size { get; set; }

            /// <summary>
            /// Gets/sets the optional width.
            /// </summary>
            public int? Width { get; set; }

            /// <summary>
            /// Gets/sets the optional height.
            /// </summary>
            public int? Height { get; set; }

            /// <summary>
            /// An optional version url for a different size when requested via the list api call on the MediaApiController. Only use when expecting this call.
            /// </summary>
            public string AltVersionUrl { get; set; }

            /// <summary>
            /// Gets/sets the last modification date.
            /// </summary>
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
        /// Gets/sets the optinal folder name
        /// </summary>
        public string CurrentFolderName { get; set; }

        /// <summary>
        /// Gets/sets the optional parent id.
        /// </summary>
        public Guid? ParentFolderId { get; set; }

        /// <summary>
        /// Gets/sets if the current folder can be deleted.
        /// </summary>
        public bool CanDelete { get; set; }

        /// <summary>
        /// Gets/sets the optional status message from the last operation.
        /// </summary>
        public StatusMessage Status { get; set; }

        /// <summary>
        /// Gets/sets the recommended view mode for the folder.
        /// </summary>
        public string ViewMode { get; set; }

        /// <summary>
        /// Gets/sets the media folder structure.
        /// </summary>
        public Piranha.Models.MediaStructure Structure { get; set; }

        /// <summary>
        /// Gets/sets the amount of media files at root level.
        /// </summary>
        public int RootCount { get; set; }

        /// <summary>
        /// Gets/sets the total amount of media files.
        /// </summary>
        public int TotalCount { get; set; }
    }
}