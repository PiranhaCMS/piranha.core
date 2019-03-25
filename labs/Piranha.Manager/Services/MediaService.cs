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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Piranha.Models;
using Piranha.Manager.Models;

namespace Piranha.Manager.Services
{
    public class MediaService
    {
        private readonly IApi _api;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The default api</param>
        public MediaService(IApi api)
        {
            _api = api;
        }

        /// <summary>
        /// Get media model by media id
        /// </summary>
        /// <param name="id">Media IDidparam>
        /// <returns>Model</returns>
        public async Task<MediaListModel.MediaItem> GetById(Guid id)
        {
            var media = await _api.Media.GetByIdAsync(id);
            if (media == null)
                return null;

            return new MediaListModel.MediaItem
            {
                Id = media.Id,
                Type = media.Type.ToString(),
                Filename = media.Filename,
                PublicUrl = media.PublicUrl.Replace("~", ""),
                ContentType = media.ContentType,
                Size = Utils.FormatByteSize(media.Size),
                Width = media.Width,
                Height = media.Height,
                LastModified = media.LastModified.ToString("yyyy-MM-dd")
            };
        }

        /// <summary>
        /// Gets the list model for the specified folder, or the root
        /// folder if to folder id is given.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        /// <returns>The list model</returns>
            public async Task<MediaListModel> GetList(Guid? folderId = null)
        {
            var model = new MediaListModel
            {
                CurrentFolderId = folderId,
                ParentFolderId = null
            };

            if (folderId.HasValue)
            {
                var folder = await _api.Media.GetFolderByIdAsync(folderId.Value);
                if (folder != null)
                {
                    model.ParentFolderId = folder.ParentId;
                }
            }
            model.Media = (await _api.Media.GetAllAsync(folderId))
                .Select(m => new MediaListModel.MediaItem
                {
                    Id = m.Id,
                    Type = m.Type.ToString(),
                    Filename = m.Filename,
                    PublicUrl = m.PublicUrl.Replace("~", ""),
                    ContentType = m.ContentType,
                    Size = Utils.FormatByteSize(m.Size),
                    Width = m.Width,
                    Height = m.Height,
                    LastModified = m.LastModified.ToString("yyyy-MM-dd")
                }).ToList();

            var structure = await _api.Media.GetStructureAsync();
            model.Folders = structure.GetPartial(folderId)
                .Select(f => new MediaListModel.FolderItem
                {
                    Id = f.Id,
                    Name = f.Name
                }).ToList();

            foreach (var folder in model.Folders)
            {
                //
                // TODO: Optimize, we don't need all this data
                //
                folder.ItemCount = (await _api.Media.GetAllAsync(folder.Id)).Count();
            }
            return model;
        }

        public async Task SaveFolder(MediaFolderModel model)
        {
            await _api.Media.SaveFolderAsync(new MediaFolder
            {
                ParentId = model.ParentId,
                Name = model.Name
            });
        }
    }
}