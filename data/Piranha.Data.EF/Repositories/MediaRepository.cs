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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Piranha.Data;
using Piranha.Data.EF;

namespace Piranha.Repositories
{
    public class MediaRepository : IMediaRepository
    {
        class FolderCount
        {
            public Guid? FolderId { get; set; }
            public int Count { get; set; }
        }

        private readonly IDb _db;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        public MediaRepository(IDb db)
        {
            _db = db;
        }

        /// <summary>
        /// Gets all media available in the specified folder.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        /// <returns>The available media</returns>
        public async Task<IEnumerable<Guid>> GetAll(Guid? folderId = null) =>
            await _db.Media
                .AsNoTracking()
                .Where(m => m.FolderId == folderId)
                .OrderBy(m => m.Filename)
                .Select(m => m.Id)
                .ToListAsync()
                .ConfigureAwait(false);

        /// <summary>
        /// <inheritdoc cref="IMediaRepository.CountAll"/>
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        public Task<int> CountAll(Guid? folderId) =>
            _db.Media
                .AsNoTracking()
                .Where(m => m.FolderId == folderId).CountAsync();

        /// <summary>
        /// Gets all media folders available in the specified
        /// folder.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        /// <returns>The available media folders</returns>
        public async Task<IEnumerable<Guid>> GetAllFolders(Guid? folderId = null) =>
            await _db.MediaFolders
                .AsNoTracking()
                .Where(f => f.ParentId == folderId)
                .OrderBy(f => f.Name)
                .Select(f => f.Id)
                .ToListAsync()
                .ConfigureAwait(false);

        /// <summary>
        /// Get media for all Ids in this enumerable.
        /// </summary>
        /// <param name="ids">One or several media id</param>
        /// <returns>The matching media</returns>
        public Task<IEnumerable<Models.Media>> GetById(params Guid[] ids) => _db.Media.AsNoTracking()
            .Include(c => c.Versions).Where(m => ids.Contains(m.Id)).OrderBy(m => m.Filename).ToArrayAsync()
            .ContinueWith(t => t.Result.Select(m => (Models.Media) m));

        /// <summary>
        /// Gets the media with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The media</returns>
        public Task<Models.Media> GetById(Guid id) =>
            _db.Media
                .AsNoTracking()
                .Include(m => m.Versions)
                .FirstOrDefaultAsync(m => m.Id == id).ContinueWith(t => (Models.Media)t.Result);

        /// <summary>
        /// Gets the media folder with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The media folder</returns>
        public Task<Models.MediaFolder> GetFolderById(Guid id) =>
            _db.MediaFolders
                .AsNoTracking()
                .Select(f => new Models.MediaFolder
                {
                    Id = f.Id,
                    ParentId = f.ParentId,
                    Name = f.Name,
                    Created = f.Created
                })
                .FirstOrDefaultAsync(f => f.Id == id);

        /// <summary>
        /// Gets the hierachical media structure.
        /// </summary>
        /// <returns>The media structure</returns>
        public async Task<Models.MediaStructure> GetStructure()
        {
            var folders = await _db.MediaFolders
                .AsNoTracking()
                .OrderBy(f => f.ParentId)
                .ThenBy(f => f.Name)
                .ToListAsync()
                .ConfigureAwait(false);

            var count = await _db.Media
                .AsNoTracking()
                .GroupBy(m => m.FolderId)
                .Select(m => new FolderCount
                {
                    FolderId = m.Key,
                    Count = m.Count()
                })
                .ToListAsync()
                .ConfigureAwait(false);

            return Sort(folders, count);
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model to save</param>
        public async Task Save(Models.Media model)
        {
            var media = await _db.Media
                .Include(m => m.Versions)
                .FirstOrDefaultAsync(m => m.Id == model.Id)
                .ConfigureAwait(false);

            if (media == null)
            {
                media = new Media()
                {
                    Id = model.Id,
                    Created = DateTime.Now
                };
                await _db.Media.AddAsync(media).ConfigureAwait(false);
            }

            media.Filename = model.Filename;
            media.FolderId = model.FolderId;
            media.Type = model.Type;
            media.Size = model.Size;
            media.Width = model.Width;
            media.Height = model.Height;
            media.ContentType = model.ContentType;
            media.Title = model.Title;
            media.AltText = model.AltText;
            media.Description = model.Description;
            media.Properties = Media.SerializeProperties(model.Properties);
            media.LastModified = DateTime.Now;

            // Delete removed versions
            var current = model.Versions.Select(v => v.Id).ToArray();
            var removed = media.Versions.Where(v => !current.Contains(v.Id)).ToArray();

            if (removed.Length > 0)
            {
                _db.MediaVersions.RemoveRange(removed);
            }

            // Add new versions
            foreach (var version in model.Versions)
            {
                if (media.Versions.All(v => v.Id != version.Id))
                {
                    var mediaVersion = new MediaVersion
                    {
                        Id = version.Id,
                        MediaId = media.Id,
                        Size = version.Size,
                        Width = version.Width,
                        Height = version.Height,
                        FileExtension = version.FileExtension
                    };
                    _db.MediaVersions.Add(mediaVersion);
                    media.Versions.Add(mediaVersion);
                }
            }

            // Save all changes
            await _db.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        public async Task SaveFolder(Models.MediaFolder model)
        {
            var folder = await _db.MediaFolders
                .FirstOrDefaultAsync(f => f.Id == model.Id)
                .ConfigureAwait(false);

            if (folder == null)
            {
                folder = new Data.MediaFolder()
                {
                    Id = model.Id != Guid.Empty ? model.Id : Guid.NewGuid(),
                    Created = DateTime.Now
                };
                model.Id = folder.Id;
                await _db.MediaFolders.AddAsync(folder).ConfigureAwait(false);
            }
            folder.ParentId = model.ParentId;
            folder.Name = model.Name;

            await _db.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Moves the media to the folder with the specified id.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="folderId">The folder id</param>
        public async Task Move(Models.Media model, Guid? folderId)
        {
            var media = await _db.Media
                .FirstOrDefaultAsync(m => m.Id == model.Id)
                .ConfigureAwait(false);

            if (media != null)
            {
                media.FolderId = folderId;
                await _db.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Deletes the media with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task Delete(Guid id)
        {
            var media = await _db.Media
                .Include(m => m.Versions)
                .FirstOrDefaultAsync(m => m.Id == id)
                .ConfigureAwait(false);

            if (media != null)
            {
                _db.Media.Remove(media);
                await _db.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Deletes the media folder with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task DeleteFolder(Guid id)
        {
            var folder = await _db.MediaFolders
                .FirstOrDefaultAsync(f => f.Id == id)
                .ConfigureAwait(false);

            if (folder != null)
            {
                _db.MediaFolders.Remove(folder);
                await _db.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Sorts the items.
        /// </summary>
        /// <param name="folders">The full folder list</param>
        /// <param name="count">The list of item count</param>
        /// <param name="parentId">The current parent id</param>
        /// <param name="level">The current level in the structure</param>
        /// <returns>The structure</returns>
        private Models.MediaStructure Sort(IEnumerable<MediaFolder> folders, IList<FolderCount> count, Guid? parentId = null, int level = 0)
        {
            var rootCount = count.FirstOrDefault(c => c.FolderId == null)?.Count;
            var totalCount = count.Sum(c => c.Count);
            var result = new Models.MediaStructure
            {
                MediaCount = rootCount.HasValue ? rootCount.Value : 0,
                TotalCount = totalCount
            };

            var mediaFolders = folders as MediaFolder[] ?? folders.ToArray();
            foreach (var folder in mediaFolders.Where(f => f.ParentId == parentId).OrderBy(f => f.Name))
            {
                var item = Module.Mapper.Map<MediaFolder, Models.MediaStructureItem>(folder);
                var folderCount = count.FirstOrDefault(c => c.FolderId == folder.Id)?.Count;

                item.Level = level;
                item.Items = Sort(mediaFolders, count, folder.Id, level + 1);
                item.FolderCount = folders.Count(f => f.ParentId == item.Id);
                item.MediaCount = folderCount.HasValue ? folderCount.Value : 0;

                result.Add(item);
            }
            return result;
        }
    }
}