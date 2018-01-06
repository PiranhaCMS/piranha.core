/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Piranha.Areas.Manager.Models
{
    public class MediaListModel
    {
        public IList<Data.Media> Media { get; set; }
        public MediaStructure Folders { get; set; }
        public IList<MediaStructureItem> Breadcrumb { get; set; }
        public Guid? CurrentFolderId { get; set; }
        public Guid? ParentFolderId { get; set; }
        public MediaType? Filter { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MediaListModel() {
            Media = new List<Data.Media>();
            Folders = new MediaStructure();
        }

        /// <summary>
        /// Gets all available content in the specified folder.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="folderId">The optional folder id</param>
        /// <param name="type">The optional media type</param>
        /// <returns>The model</returns>
        public static MediaListModel Get(IApi api, Guid? folderId = null, MediaType? type = null) {
            var model = new MediaListModel() {
                CurrentFolderId = folderId,
                ParentFolderId = null,
                Filter = type
            };

            if (folderId.HasValue) {
                var folder = api.Media.GetFolderById(folderId.Value);
                if (folder != null)
                    model.ParentFolderId = folder.ParentId;
            }                
            model.Media = api.Media.GetAll(folderId).ToList();

            if (type.HasValue)
                model.Media = model.Media.Where(m => m.Type == type.Value).ToList();

            var structure = api.Media.GetStructure();
            model.Folders = structure.GetPartial(folderId);
            model.Breadcrumb = structure.GetBreadcrumb(folderId);

            return model;
        }
    }
}