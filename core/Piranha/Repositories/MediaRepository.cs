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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piranha.Repositories
{
    public class MediaRepository : IMediaRepository
    {
        private readonly IDb db;
        private readonly Api api;
        private readonly IStorage storage;
        private readonly ICache cache;   
        private readonly IImageProcessor processor;
        private static object scalemutex = new object();     
        private const string MEDIA_STRUCTURE = "MediaStructure";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        /// <param name="storage">The current storage manager</param>
        /// <param name="cache">The optional model cache</param>
        public MediaRepository(Api api, IDb db, IStorage storage, ICache cache = null, IImageProcessor processor = null) {
            this.api = api;
            this.db = db;
            this.storage = storage;
            this.cache = cache;
            this.processor = processor;
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

                if (model != null) {
                    model.PublicUrl = GetPublicUrl(model);
                    App.Hooks.OnLoad<Media>(model);
                }

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

                if (model != null)
                    App.Hooks.OnLoad<MediaFolder>(model);

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
                .Include(m => m.Versions)
                .FirstOrDefault(m => m.Id == content.Id);

            if (model == null) {
                model = new Media() {
                    Id = model != null || content.Id.HasValue ? content.Id.Value : Guid.NewGuid(),
                    Created = DateTime.Now
                };
                content.Id = model.Id;
                db.Media.Add(model);
            } else {
                using (var session = await storage.OpenAsync()) {
                    // Delete all versions as we're updating the image
                    if (model.Versions.Count > 0) {
                        foreach (var version in model.Versions) {
                            // Delete version from storage
                            await session.DeleteAsync(GetResourceName(model, version.Width, version.Height, version.FileExtension));
                        }
                        db.MediaVersions.RemoveRange(model.Versions);
                    }

                    // Delete the old file because we might have a different filename
                    await session.DeleteAsync(GetResourceName(model));
                }
            }

            model.Filename = content.Filename;
            model.FolderId = content.FolderId;
            model.Type = App.MediaTypes.GetMediaType(content.Filename);
            model.ContentType = App.MediaTypes.GetContentType(content.Filename);
            model.LastModified = DateTime.Now;

            // Pre-process if this is an image
            if (processor != null && model.Type == Models.MediaType.Image) {
                byte[] bytes;

                if (content is Models.BinaryMediaContent) {
                    bytes = ((Models.BinaryMediaContent)content).Data;
                } else {
                    var reader = new BinaryReader(((Models.StreamMediaContent)content).Data);
                    bytes = reader.ReadBytes((int)reader.BaseStream.Length);
                    ((Models.StreamMediaContent)content).Data.Position = 0;
                }

                int width, height;
                
                processor.GetSize(bytes, out width, out height);
                model.Width = width;
                model.Height = height;
            }

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
        /// Moves the media to the folder with the specified id.
        /// </summary>
        /// <param name="media">The media</param>
        /// <param name="folderId">The folder id</param>
        public void Move(Media model, Guid? folderId) {
            var media = db.Media.FirstOrDefault(m => m.Id == model.Id);
            if (media != null) {
                media.FolderId = folderId;
                db.SaveChanges();

                RemoveFromCache(media);
            }
        }

        /// <summary>
        /// Ensures that the image version with the given size exsists
        /// and returns its public URL. Please note that this method is 
        /// not really synchronous, it's just a wrapper for the async version.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="width">The requested width</param>
        /// <param name="height">The optionally requested height</param>
        /// <returns>The public URL</returns>
        public string EnsureVersion(Guid id, int width, int? height = null) {
            var task = Task.Run(() => EnsureVersionAsync(id, width, height));
            task.Wait();
            return task.Result;
        }

        /// <summary>
        /// Ensures that the image version with the given size exsists
        /// and returns its public URL.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="width">The requested width</param>
        /// <param name="height">The optionally requested height</param>
        /// <returns>The public URL</returns>
        public async Task<string> EnsureVersionAsync(Guid id, int width, int? height = null) {
            if (processor != null) {
                var query = db.MediaVersions
                    .Where(v => v.MediaId == id && v.Width == width);

                if (height.HasValue)
                    query = query.Where(v => v.Height == height);
                else query = query.Where(v => !v.Height.HasValue);

                var version = query.FirstOrDefault();

                if (version == null) {
                    var media = db.Media
                        .FirstOrDefault(m => m.Id == id);

                    // If the media is missing return false
                    if (media == null)
                        return null;

                    // If the requested size is equal to the original size, return true
                    if (media.Width == width && (!height.HasValue || media.Height == height.Value))
                        return GetPublicUrl(media);

                    // Get the image file
                    using (var stream = new MemoryStream()) {
                        using (var session = await storage.OpenAsync()) {
                            if (!await session.GetAsync(media.Id + "-" + media.Filename, stream))
                                return null;

                            // Reset strem position    
                            stream.Position = 0;

                            using (var output = new MemoryStream()) {
                                if (height.HasValue) {
                                    processor.CropScale(stream, output, width, height.Value);
                                } else {
                                    processor.Scale(stream, output, width);
                                }
                                output.Position = 0;
                                bool upload = false;

                                lock (scalemutex) {
                                    // We have to make sure we don't scale multiple files
                                    // at the same time as it can create index violations.
                                    version = query.FirstOrDefault();

                                    if (version == null) {
                                        var info = new FileInfo(media.Filename);

                                        version = new MediaVersion() {
                                            Id = Guid.NewGuid(),
                                            MediaId = media.Id,
                                            Size = output.Length,
                                            Width = width,
                                            Height = height,
                                            FileExtension = info.Extension
                                        };
                                        db.MediaVersions.Add(version);
                                        db.SaveChanges();

                                        upload = true;
                                    }
                                }

                                if (upload) {
                                    return await session.PutAsync(GetResourceName(media, width, height), media.ContentType, output);
                                }
                            }
                        }
                    }
                } else {
                    var media = db.Media
                        .FirstOrDefault(m => m.Id == id);

                    // If the media is missing return false
                    if (media == null)
                        return null;

                    // If the requested size is equal to the original size, return true
                    if (media.Width == width && (!height.HasValue || media.Height == height.Value))
                        return GetPublicUrl(media);
                    return GetPublicUrl(media, width, height, version.FileExtension);
                }
            }
            return null;
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
                .Include(m => m.Versions)
                .FirstOrDefault(m => m.Id == id);

            if (media != null) {
                using (var session = await storage.OpenAsync()) {
                    if (media.Versions.Count > 0) {
                        foreach (var version in media.Versions) {
                            // Delete version from storage
                            await session.DeleteAsync(GetResourceName(media, version.Width, version.Height, version.FileExtension));
                        }
                        db.MediaVersions.RemoveRange(media.Versions);
                    }
                    App.Hooks.OnBeforeDelete<Media>(media);

                    db.Media.Remove(media);
                    db.SaveChanges();

                    // Delete from storage
                    await session.DeleteAsync(media.Id + "-" + media.Filename);
                }
                App.Hooks.OnAfterDelete<Media>(media);

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

        /// <summary>
        /// Gets the media resource name.
        /// </summary>
        /// <param name="media">The media object</param>
        /// <param name="width">Optional requested width</param>
        /// <param name="height">Optional requested height</param>
        /// <param name="extension">Optional requested extension</param>
        /// <returns>The name</returns>
        private string GetResourceName(Media media, int? width = null, int? height = null, string extension = null) {
            var filename = new FileInfo(media.Filename);
            var sb = new StringBuilder();

            sb.Append(media.Id);
            sb.Append("-");

            if (width.HasValue) {
                sb.Append(filename.Name.Replace(filename.Extension, "_"));
                sb.Append(width);

                if (height.HasValue) {
                    sb.Append("x");
                    sb.Append(height.Value);
                }
            } else {
                sb.Append(filename.Name.Replace(filename.Extension, ""));
            }

            if (string.IsNullOrEmpty(extension))
                sb.Append(filename.Extension);
            else sb.Append(extension);

            return sb.ToString();
        }

        /// <summary>
        /// Gets the public url for the given media.
        /// </summary>
        /// <param name="media">The media object</param>
        /// <param name="width">Optional requested width</param>
        /// <param name="height">Optional requested height</param>
        /// <param name="extension">Optional requested extension</param>
        /// <returns>The name</returns>
        private string GetPublicUrl(Media media, int? width = null, int? height = null, string extension = null) {
            var name = GetResourceName(media, width, height, extension);

            using (var config = new Config(api)) {
                var cdn = config.MediaCDN;

                if (!string.IsNullOrWhiteSpace(cdn))
                    return cdn + name;
                return storage.GetPublicUrl(name);
            }
        }
    }
}