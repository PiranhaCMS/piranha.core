/*
 * Copyright (c) 2017-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Piranha.Data;
using Piranha.Data.EF;

namespace Piranha.Repositories
{
    public class MediaRepository : IMediaRepository
    {
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
        public async Task<IEnumerable<Guid>> GetAll(Guid? folderId = null)
        {
            return await _db.Media
                .AsNoTracking()
                .Where(m => m.FolderId == folderId)
                .OrderBy(m => m.Filename)
                .Select(m => m.Id)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all media folders available in the specified
        /// folder.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        /// <returns>The available media folders</returns>
        public async Task<IEnumerable<Guid>> GetAllFolders(Guid? folderId = null)
        {
            return await _db.MediaFolders
                .AsNoTracking()
                .Where(f => f.ParentId == folderId)
                .OrderBy(f => f.Name)
                .Select(f => f.Id)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the media with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The media</returns>
        public Task<Models.Media> GetById(Guid id)
        {
            return _db.Media
                .AsNoTracking()
                .Include(m => m.Versions)
                .Select(m => new Models.Media
                {
                    Id = m.Id,
                    FolderId = m.FolderId,
                    Type = m.Type,
                    Filename = m.Filename,
                    ContentType = m.ContentType,
                    Size = m.Size,
                    Width = m.Width,
                    Height = m.Height,
                    Created = m.Created,
                    LastModified = m.LastModified,
                    Versions = m.Versions.Select(v => new Models.MediaVersion
                    {
                        Id = v.Id,
                        Size = v.Size,
                        Width = v.Width,
                        Height = v.Height,
                        FileExtension = v.FileExtension
                    }).ToList()
                })
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        /// <summary>
        /// Gets the media folder with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The media folder</returns>
        public Task<Models.MediaFolder> GetFolderById(Guid id)
        {
            return _db.MediaFolders
                .AsNoTracking()
                .Select(f => new Models.MediaFolder
                {
                    Id = f.Id,
                    ParentId = f.ParentId,
                    Name = f.Name,
                    Created = f.Created
                })
                .FirstOrDefaultAsync(f => f.Id == id);
        }

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

            return Sort(folders);
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="content">The content to save</param>
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
                if (!media.Versions.Any(v => v.Id == version.Id))
                {
                    media.Versions.Add(new MediaVersion
                    {
                        Id = version.Id,
                        MediaId = media.Id,
                        Size = version.Size,
                        Width = version.Width,
                        Height = version.Height,
                        FileExtension = version.FileExtension
                    });
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
        /// <param name="media">The media</param>
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
        /// <param name="parentId">The current parent id</param>
        /// <returns>The structure</returns>
        private Models.MediaStructure Sort(IEnumerable<MediaFolder> folders, Guid? parentId = null, int level = 0)
        {
            var result = new Models.MediaStructure();

            foreach (var folder in folders.Where(f => f.ParentId == parentId).OrderBy(f => f.Name))
            {
                var item = Module.Mapper.Map<MediaFolder, Models.MediaStructureItem>(folder);

                item.Level = level;
                item.Items = Sort(folders, folder.Id, level + 1);

                result.Add(item);
            }
            return result;
        }
    }
}