/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.EntityFrameworkCore;
using Piranha.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Piranha.Repositories
{
    public class MediaRepository : IMediaRepository
    {
        private readonly IDb db;
        private readonly IStorage storage;
        private readonly ICache cache;        

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        /// <param name="storage">The current storage manager</param>
        /// <param name="cache">The optional model cache</param>
        public MediaRepository(IDb db, IStorage storage, ICache cache = null) {
            this.db = db;
            this.storage = storage;
            this.cache = cache;
        }

        /// <summary>
        /// Gets all media available in the specified folder.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        /// <returns>The available media</returns>
        public IEnumerable<Media> GetAll(Guid? folderId = null) {
            IEnumerable<Media> models;

            models = db.Media
                .AsNoTracking()
                .Where(m => m.FolderId == folderId)
                .OrderBy(m => m.Filename)
                .ToList();

            foreach (var model in models) {
                model.PublicUrl = storage.GetPublicUrl(model);
            }
            return models;
        }

        /// <summary>
        /// Gets all media folders available in the specified
        /// folder.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        /// <returns>The available media folders</returns>
        public IEnumerable<MediaFolder> GetAllFolders(Guid? folderId = null) {
            return db.MediaFolders
                .AsNoTracking()
                .Where(f => f.ParentId == folderId)
                .OrderBy(f => f.Name)
                .ToList();
        }

        /// <summary>
        /// Gets the media with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The media</returns>
        public Media GetById(Guid id) {
            Media model = cache != null ? cache.Get<Media>(id.ToString()) : null;

            if (model == null) {
                model = db.Media
                    .AsNoTracking()
                    .FirstOrDefault(m => m.Id == id);

                if (model != null)
                    model.PublicUrl = storage.GetPublicUrl(model);

                if (cache != null && model != null)
                    cache.Set(model.Id.ToString(), model);                
            }
            return model;
        }

        /// <summary>
        /// Gets the media folder with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The media folder</returns>
        public MediaFolder GetFolderById(Guid id) {
            MediaFolder model = cache != null ? cache.Get<MediaFolder>(id.ToString()) : null;

            if (model == null) {
                model = db.MediaFolders
                    .AsNoTracking()
                    .FirstOrDefault(f => f.Id == id);

                if (cache != null && model != null)
                    cache.Set(model.Id.ToString(), model);                                
            }
            return model;
        }

        /// <summary>
        /// Gets the hierachical media structure.
        /// </summary>
        /// <returns>The media structure</returns>
        public Models.MediaStructure GetStructure() {
            Models.MediaStructure model = cache != null ? cache.Get<Models.MediaStructure>("MediaStructure") : null;

            if (model == null) {
                var folders = db.MediaFolders
                    .AsNoTracking()
                    .OrderBy(f => f.ParentId)
                    .ThenBy(f => f.Name)
                    .ToList();

                model = Sort(folders);

                if (cache != null && model != null)
                    cache.Set("MediaStructure", model);  
            }
            return model;
        }

        /// <summary>
        /// Adds or updates the given model in the database depending on its state.
        /// Please note that this method is not really synchronous, it's just a 
        /// wrapper for the async version.
        /// </summary>
        /// <param name="content">The content to save</param>
        public void Save(Models.MediaContent content) {
            Task.Run(() => SaveAsync(content)).Wait();
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="content">The content to save</param>
        public async Task SaveAsync(Models.MediaContent content) {
            if (!App.MediaTypes.IsSupported(content.Filename))
                throw new NotSupportedException("Filetype not supported.");

            var model = db.Media
                .FirstOrDefault(m => m.Id == content.Id);

            if (model == null) {
                model = new Media() {
                    Id = model != null || content.Id.HasValue ? content.Id.Value : Guid.NewGuid(),
                    Created = DateTime.Now
                };
                content.Id = model.Id;
                db.Media.Add(model);
            }

            model.Filename = content.Filename;
            model.FolderId = content.FolderId;
            model.Type = App.MediaTypes.GetMediaType(content.Filename);
            model.ContentType = App.MediaTypes.GetContentType(content.Filename);
            model.LastModified = DateTime.Now;

            // Upload to storage
            using (var session = await storage.OpenAsync()) {
                if (content is Models.BinaryMediaContent) {
                    var bc = (Models.BinaryMediaContent)content;

                    model.Size = bc.Data.Length;
                    await session.PutAsync(model.Id + "-" + model.Filename, 
                        model.ContentType, bc.Data);
                } else if (content is Models.StreamMediaContent) {
                    var sc = (Models.StreamMediaContent)content;
                    var stream = sc.Data;

                    model.Size = sc.Data.Length;
                    await session.PutAsync(model.Id + "-" + model.Filename, 
                        model.ContentType, stream);
                }
            }

            db.SaveChanges();
            RemoveFromCache(model);
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        public void SaveFolder(Data.MediaFolder model) {
            var folder = db.MediaFolders
                .FirstOrDefault(f => f.Id == model.Id);

            if (folder == null) {
                folder = new Data.MediaFolder() {
                    Id = model.Id != Guid.Empty ? model.Id : Guid.NewGuid(),
                    Created = DateTime.Now
                };
                model.Id = folder.Id;
                db.MediaFolders.Add(folder);
            }

            App.Mapper.Map<Data.MediaFolder, Data.MediaFolder>(model, folder);

            db.SaveChanges();
            RemoveFromCache(folder);
            RemoveStructureFromCache();
        }

        /// <summary>
        /// Deletes the media with the given id. Please note that this method
        /// is not really synchronous, it's just a wrapper for the async version.
        /// </summary>
        /// <param name="id">The unique id</param>
        public void Delete(Guid id) {
            Task.Run(() => DeleteAsync(id)).Wait();
        }

        /// <summary>
        /// Deletes the media with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task DeleteAsync(Guid id) {
            var media = db.Media
                .FirstOrDefault(m => m.Id == id);

            if (media != null) {
                db.Media.Remove(media);
                db.SaveChanges();

                // Delete from storage
                using (var session = await storage.OpenAsync()) {
                    await session.DeleteAsync(media.Id + "-" + media.Filename);
                }

                RemoveFromCache(media);
            }
        }

        /// <summary>
        /// Deletes the given model. Please note that this method
        /// is not really synchronous, it's just a wrapper for the async version.
        /// </summary>
        /// <param name="model">The media</param>
        public void Delete(Media model) {
            Delete(model.Id);
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The media</param>
        public async Task DeleteAsync(Media model) {
            await DeleteAsync(model.Id);
        }

        /// <summary>
        /// Deletes the media folder with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public void DeleteFolder(Guid id) {
            var folder = db.MediaFolders
                .FirstOrDefault(f => f.Id == id);

            if (folder != null) {
                var folderCount = db.MediaFolders.Count(f => f.ParentId == id);
                var mediaCount = db.Media.Count(m => m.FolderId == id);

                if (folderCount == 0 && mediaCount == 0) {
                    db.MediaFolders.Remove(folder);
                    db.SaveChanges();

                    RemoveFromCache(folder);
                }
            }
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The media</param>
        public void DeleteFolder(Data.MediaFolder model) {
            DeleteFolder(model.Id);
        }

        /// <summary>
        /// Sorts the items.
        /// </summary>
        /// <param name="folders">The full folder list</param>
        /// <param name="parentId">The current parent id</param>
        /// <returns>The structure</returns>
        private Models.MediaStructure Sort(IEnumerable<MediaFolder> folders, Guid? parentId = null, int level = 0) {
            var result = new Models.MediaStructure();

            foreach (var folder in folders.Where(f => f.ParentId == parentId).OrderBy(f => f.Name)) {
                var item = App.Mapper.Map<MediaFolder, Models.MediaStructureItem>(folder);

                item.Level = level;
                item.Items = Sort(folders, folder.Id, level + 1);

                result.Add(item);
            }
            return result;
        }

        /// <summary>
        /// Removes the given model from cache.
        /// </summary>
        /// <param name="model">The model</param>
        private void RemoveFromCache(Data.Media model) {
            if (cache != null)
                cache.Remove(model.Id.ToString());
        }

        /// <summary>
        /// Removes the given model from cache.
        /// </summary>
        /// <param name="model">The model</param>
        private void RemoveFromCache(MediaFolder model) {
            if (cache != null) {
                cache.Remove(model.Id.ToString());
                RemoveStructureFromCache();
            }
        }

        /// <summary>
        /// Removes the given model from cache.
        /// </summary>
        /// <param name="model">The model</param>
        private void RemoveStructureFromCache() {
            if (cache != null)
                cache.Remove("MediaStructure");
        }
    }
}