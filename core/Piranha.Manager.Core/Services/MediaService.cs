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
using System.Linq;
using System.Threading.Tasks;
using Piranha.Models;
using Piranha.Manager.Models;
using System.IO;

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
                FolderId = media.FolderId,
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
        public async Task<MediaListModel> GetList(Guid? folderId = null, MediaType? filter = null, int? width = null, int? height = null)
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
                    model.CurrentFolderName = folder.Name;
                    model.ParentFolderId = folder.ParentId;
                }
            }

            var holdMedia = (await _api.Media.GetAllByFolderIdAsync(folderId));
            if (filter.HasValue)
            {
                holdMedia = holdMedia
                    .Where(m => m.Type == filter.Value);
            }
            var pairMedia = holdMedia.Select(m => new { media = m, mediaItem = new MediaListModel.MediaItem
                {
                    Id = m.Id,
                    FolderId = m.FolderId,
                    Type = m.Type.ToString(),
                    Filename = m.Filename,
                    PublicUrl = m.PublicUrl.TrimStart('~'), //Will only enumerate the start of the string, probably a faster operation.
                    ContentType = m.ContentType,
                    Size = Utils.FormatByteSize(m.Size),
                    Width = m.Width,
                    Height = m.Height,
                    LastModified = m.LastModified.ToString("yyyy-MM-dd")
                }}).ToArray();

            var structure = await _api.Media.GetStructureAsync();
            model.Folders = structure.GetPartial(folderId)
                .Select(f => new MediaListModel.FolderItem
                {
                    Id = f.Id,
                    Name = f.Name
                }).ToList();

            foreach (var folder in model.Folders)
                folder.ItemCount = await _api.Media.CountFolderItemsAsync(folder.Id) +
                                   structure.GetPartial(folder.Id).Count;
            if (width.HasValue)
                foreach (var mp in pairMedia.Where(m => m.media.Type == MediaType.Image))
                {
                    if (mp.media.Versions.Any(v => v.Width == width && v.Height == height))
                        mp.mediaItem.AltVersionUrl =
                            (await _api.Media.EnsureVersionAsync(mp.media, width.Value, height).ConfigureAwait(false))
                            .TrimStart('~');
                }

            model.Media = pairMedia.Select(m => m.mediaItem).ToList();
            model.ViewMode = model.Media.Count(m => m.Type == "Image") > model.Media.Count / 2 ? MediaListModel.GalleryView : MediaListModel.ListView;

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

        public async Task<Guid?> DeleteFolder(Guid id)
        {
            var folder = await _api.Media.GetFolderByIdAsync(id);

            if (folder != null)
            {
                await _api.Media.DeleteFolderAsync(folder);
                return folder.ParentId;
            }
            return null;
        }

        /// <summary>
        /// Save or update media assets to storage
        /// </summary>
        /// <param name="model">Upload model</param>
        /// <returns>The number of upload managed to be saved or updated</returns>
        public async Task<int> SaveMedia(MediaUploadModel model)
        {
            var uploaded = 0;

            // Go through all of the uploaded files
            foreach (var upload in model.Uploads)
            {
                if (upload.Length > 0 && !string.IsNullOrWhiteSpace(upload.ContentType))
                {
                    using (var stream = upload.OpenReadStream())
                    {
                        await _api.Media.SaveAsync(new StreamMediaContent
                        {
                            Id = model.Uploads.Count() == 1 ? model.Id : null,
                            FolderId = model.ParentId,
                            Filename = Path.GetFileName(upload.FileName),
                            Data = stream
                        });
                        uploaded++;
                    }
                }
            }
            return uploaded;
        }

        public async Task<Guid?> DeleteMedia(Guid id)
        {
            var media = await _api.Media.GetByIdAsync(id);

            if (media != null)
            {
                await _api.Media.DeleteAsync(media);
                return media.FolderId;
            }
            return null;
        }
    }
}