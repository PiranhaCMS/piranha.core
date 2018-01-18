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

namespace Piranha.Repositories
{
    public class MediaRepository : IMediaRepository
    {
        private readonly IDb db;
        private readonly IStorage storage;
        private readonly ICache cache;
        private const string MEDIA_STRUCTURE = "MediaStructure";

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
            List<Media> models = new List<Media>();

            var media = db.Media
                .AsNoTracking()
                .Where(m => m.FolderId == folderId)
                .OrderBy(m => m.Filename)
                .Select(m => m.Id);

            foreach (var id in media) {
                var model = GetById(id);
                if (model != null) 
                    models.Add(model);
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
            List<MediaFolder> models = new List<MediaFolder>();

            var folders = db.MediaFolders
                .AsNoTracking()
                .Where(f => f.ParentId == folderId)
                .OrderBy(f => f.Name)
                .Select(f => f.Id);

            foreach (var id in folders) {
                var model = GetFolderById(id);
                if (model != null) 
                    models.Add(model);
            }
            return models;
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

                if (cache != null && model != null) {
                    App.Hooks.OnLoad<Media>(model);
                    cache.Set(model.Id.ToString(), model);
                }
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

                if (cache != null && model != null) {
                    App.Hooks.OnLoad<MediaFolder>(model);
                    cache.Set(model.Id.ToString(), model);
                }
            }
            return model;
        }

        /// <summary>
        /// Gets the hierachical media structure.
        /// </summary>
        /// <returns>The media structure</returns>
        public Models.MediaStructure GetStructure() {
            Models.MediaStructure model = cache != null ? cache.Get<Models.MediaStructure>(MEDIA_STRUCTURE) : null;

            if (model == null) {
                var folders = new List<MediaFolder>();
                var ids = db.MediaFolders
                    .AsNoTracking()
                    .OrderBy(f => f.ParentId)
                    .ThenBy(f => f.Name)
                    .Select(f => f.Id)
                    .ToList();
                
                foreach (var id in ids) {
                    var folder = GetFolderById(id);
                    if (folder != null) 
                        folders.Add(folder);
                }

                model = Sort(folders);

                if (cache != null && model != null)
                    cache.Set(MEDIA_STRUCTURE, model);  
            }
            return model;
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="data">The binary data</param>
        public void Save(Models.MediaContent content) {
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
            using (var session = storage.Open()) {
                if (content is Models.BinaryMediaContent) {
                    var bc = (Models.BinaryMediaContent)content;

                    model.Size = bc.Data.Length;
                    session.Put(model.Id + "-" + model.Filename, 
                        model.ContentType, bc.Data);
                } else if (content is Models.StreamMediaContent) {
                    var sc = (Models.StreamMediaContent)content;
                    var stream = sc.Data;

                    model.Size = sc.Data.Length;
                    session.Put(model.Id + "-" + model.Filename, 
                        model.ContentType, ref stream);
                }
            }

            App.Hooks.OnBeforeSave<Media>(model);
            db.SaveChanges();
            App.Hooks.OnAfterSave<Media>(model);

            RemoveFromCache(model);
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        public void SaveFolder(Data.MediaFolder model) {
            App.Hooks.OnBeforeSave<MediaFolder>(model);

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

            App.Hooks.OnAfterSave<MediaFolder>(model);

            RemoveFromCache(folder);
            RemoveStructureFromCache();
        }

        /// <summary>
        /// Deletes the media with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public void Delete(Guid id) {
            var media = db.Media
                .FirstOrDefault(m => m.Id == id);

            if (media != null) {
                App.Hooks.OnBeforeDelete<Media>(media);

                db.Media.Remove(media);
                db.SaveChanges();

                // Delete from storage
                using (var session = storage.Open()) {
                    session.Delete(media.Id + "-" + media.Filename);
                }
                App.Hooks.OnAfterDelete<Media>(media);

                RemoveFromCache(media);
            }
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The media</param>
        public void Delete(Media model) {
            Delete(model.Id);
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
                    App.Hooks.OnBeforeDelete<MediaFolder>(folder);

                    db.MediaFolders.Remove(folder);
                    db.SaveChanges();
                    
                    App.Hooks.OnAfterDelete<MediaFolder>(folder);

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
                cache.Remove(MEDIA_STRUCTURE);
        }
    }
}