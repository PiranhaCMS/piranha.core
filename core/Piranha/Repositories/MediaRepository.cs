/*
 * Copyright (c) 2017 Håkan Edling
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
using Piranha.Models;

namespace Piranha.Repositories
{
    public class MediaRepository : IMediaRepository
    {
        private readonly IDb _db;
        private readonly Api _api;
        private readonly IStorage _storage;
        private readonly ICache _cache;
        private readonly IImageProcessor _processor;
        private static readonly object Scalemutex = new object();
        private const string MEDIA_STRUCTURE = "MediaStructure";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        /// <param name="storage">The current storage manager</param>
        /// <param name="cache">The optional model cache</param>
        public MediaRepository(Api api, IDb db, IStorage storage, ICache cache = null, IImageProcessor processor = null)
        {
            _api = api;
            _db = db;
            _storage = storage;
            _cache = cache;
            _processor = processor;
        }

        /// <summary>
        /// Gets all media available in the specified folder.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        /// <returns>The available media</returns>
        public IEnumerable<Media> GetAll(Guid? folderId = null)
        {
            var models = new List<Media>();

            var media = _db.Media
                .AsNoTracking()
                .Where(m => m.FolderId == folderId)
                .OrderBy(m => m.Filename)
                .Select(m => m.Id);

            foreach (var id in media)
            {
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
        public IEnumerable<MediaFolder> GetAllFolders(Guid? folderId = null)
        {
            var models = new List<MediaFolder>();

            var folders = _db.MediaFolders
                .AsNoTracking()
                .Where(f => f.ParentId == folderId)
                .OrderBy(f => f.Name)
                .Select(f => f.Id);

            foreach (var id in folders)
            {
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
        public Media GetById(Guid id)
        {
            var model = _cache?.Get<Media>(id.ToString());

            if (model != null)
            {
                return model;
            }

            model = _db.Media
                .AsNoTracking()
                .FirstOrDefault(m => m.Id == id);

            if (model != null)
            {
                model.PublicUrl = GetPublicUrl(model);
                App.Hooks.OnLoad(model);
            }

            if (_cache != null && model != null)
            {
                _cache.Set(model.Id.ToString(), model);
            }

            return model;
        }

        /// <summary>
        /// Gets the media folder with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The media folder</returns>
        public MediaFolder GetFolderById(Guid id)
        {
            var model = _cache?.Get<MediaFolder>(id.ToString());

            if (model != null)
            {
                return model;
            }

            model = _db.MediaFolders
                .AsNoTracking()
                .FirstOrDefault(f => f.Id == id);

            if (model != null)
            {
                App.Hooks.OnLoad(model);
            }

            if (_cache != null && model != null)
            {
                _cache.Set(model.Id.ToString(), model);
            }

            return model;
        }

        /// <summary>
        /// Gets the hierachical media structure.
        /// </summary>
        /// <returns>The media structure</returns>
        public MediaStructure GetStructure()
        {
            var model = _cache?.Get<MediaStructure>(MEDIA_STRUCTURE);

            if (model != null)
            {
                return model;
            }

            var folders = new List<MediaFolder>();
            var ids = _db.MediaFolders
                .AsNoTracking()
                .OrderBy(f => f.ParentId)
                .ThenBy(f => f.Name)
                .Select(f => f.Id)
                .ToList();

            foreach (var id in ids)
            {
                var folder = GetFolderById(id);
                if (folder != null)
                {
                    folders.Add(folder);
                }
            }

            model = Sort(folders);

            if (_cache != null && model != null)
            {
                _cache.Set(MEDIA_STRUCTURE, model);
            }

            return model;
        }

        /// <summary>
        /// Adds or updates the given model in the database depending on its state.
        /// Please note that this method is not really synchronous, it's just a 
        /// wrapper for the async version.
        /// </summary>
        /// <param name="content">The content to save</param>
        public void Save(MediaContent content)
        {
            Task.Run(() => SaveAsync(content)).Wait();
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="content">The content to save</param>
        public async Task SaveAsync(MediaContent content)
        {
            if (!App.MediaTypes.IsSupported(content.Filename))
            {
                throw new NotSupportedException("Filetype not supported.");
            }

            var model = _db.Media
                .Include(m => m.Versions)
                .FirstOrDefault(m => m.Id == content.Id);

            if (model == null)
            {
                model = new Media
                {
                    Id = content.Id ?? Guid.NewGuid(),
                    Created = DateTime.Now
                };
                content.Id = model.Id;
                _db.Media.Add(model);
            }
            else
            {
                using (var session = await _storage.OpenAsync())
                {
                    // Delete all versions as we're updating the image
                    if (model.Versions.Count > 0)
                    {
                        foreach (var version in model.Versions)
                        {
                            // Delete version from storage
                            await session.DeleteAsync(GetResourceName(model, version.Width, version.Height, ".jpg"));
                        }
                        _db.MediaVersions.RemoveRange(model.Versions);
                    }
                }
            }

            model.Filename = content.Filename;
            model.FolderId = content.FolderId;
            model.Type = App.MediaTypes.GetMediaType(content.Filename);
            model.ContentType = App.MediaTypes.GetContentType(content.Filename);
            model.LastModified = DateTime.Now;

            // Pre-process if this is an image
            if (_processor != null && model.Type == MediaType.Image)
            {
                byte[] bytes;

                if (content is BinaryMediaContent)
                {
                    bytes = ((BinaryMediaContent)content).Data;
                }
                else
                {
                    var reader = new BinaryReader(((StreamMediaContent)content).Data);
                    bytes = reader.ReadBytes((int)reader.BaseStream.Length);
                    ((StreamMediaContent)content).Data.Position = 0;
                }

                _processor.GetSize(bytes, out var width, out var height);
                model.Width = width;
                model.Height = height;
            }

            // Upload to storage
            using (var session = await _storage.OpenAsync())
            {
                if (content is BinaryMediaContent bc)
                {
                    model.Size = bc.Data.Length;
                    await session.PutAsync(model.Id + "-" + model.Filename,
                        model.ContentType, bc.Data);
                }
                else if (content is StreamMediaContent)
                {
                    var sc = (StreamMediaContent)content;
                    var stream = sc.Data;

                    model.Size = sc.Data.Length;
                    await session.PutAsync(model.Id + "-" + model.Filename,
                        model.ContentType, stream);
                }
            }

            App.Hooks.OnBeforeSave(model);
            _db.SaveChanges();
            App.Hooks.OnAfterSave(model);

            RemoveFromCache(model);
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        public void SaveFolder(MediaFolder model)
        {
            App.Hooks.OnBeforeSave(model);

            var folder = _db.MediaFolders
                .FirstOrDefault(f => f.Id == model.Id);

            if (folder == null)
            {
                folder = new MediaFolder
                {
                    Id = model.Id != Guid.Empty ? model.Id : Guid.NewGuid(),
                    Created = DateTime.Now
                };
                model.Id = folder.Id;
                _db.MediaFolders.Add(folder);
            }

            App.Mapper.Map(model, folder);

            _db.SaveChanges();

            App.Hooks.OnAfterSave(model);

            RemoveFromCache(folder);
            RemoveStructureFromCache();
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
        public string EnsureVersion(Guid id, int width, int? height = null)
        {
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
        public async Task<string> EnsureVersionAsync(Guid id, int width, int? height = null)
        {
            if (_processor == null)
            {
                return null;
            }

            var query = _db.MediaVersions
                .Where(v => v.MediaId == id && v.Width == width);

            query = height.HasValue ? query.Where(v => v.Height == height) : query.Where(v => !v.Height.HasValue);

            var version = query.FirstOrDefault();

            if (version == null)
            {
                var media = _db.Media
                    .FirstOrDefault(m => m.Id == id);

                // If the media is missing return false
                if (media == null)
                {
                    return null;
                }

                // If the requested size is equal to the original size, return true
                if (media.Width == width && (!height.HasValue || media.Height == height.Value))
                {
                    return GetPublicUrl(media);
                }

                // Get the image file
                using (var stream = new MemoryStream())
                {
                    using (var session = await _storage.OpenAsync())
                    {
                        if (!await session.GetAsync(media.Id + "-" + media.Filename, stream))
                        {
                            return null;
                        }

                        // Reset strem position    
                        stream.Position = 0;

                        using (var output = new MemoryStream())
                        {
                            if (height.HasValue)
                            {
                                _processor.CropScale(stream, output, width, height.Value);
                            }
                            else
                            {
                                _processor.Scale(stream, output, width);
                            }
                            output.Position = 0;
                            var upload = false;

                            lock (Scalemutex)
                            {
                                // We have to make sure we don't scale multiple files
                                // at the same time as it can create index violations.
                                version = query.FirstOrDefault();

                                if (version == null)
                                {
                                    version = new MediaVersion
                                    {
                                        Id = Guid.NewGuid(),
                                        MediaId = media.Id,
                                        Size = output.Length,
                                        Width = width,
                                        Height = height
                                    };
                                    _db.MediaVersions.Add(version);
                                    _db.SaveChanges();

                                    upload = true;
                                }
                            }

                            if (upload)
                            {
                                return await session.PutAsync(GetResourceName(media, width, height, ".jpg"), "image/jpeg", output);
                            }
                        }
                    }
                }
            }
            else
            {
                var media = _db.Media
                    .FirstOrDefault(m => m.Id == id);

                // If the media is missing return false
                if (media == null)
                {
                    return null;
                }

                // If the requested size is equal to the original size, return true
                if (media.Width == width && (!height.HasValue || media.Height == height.Value))
                {
                    return GetPublicUrl(media);
                }

                return GetPublicUrl(media, width, height, ".jpg");
            }

            return null;
        }

        /// <summary>
        /// Deletes the media with the given id. Please note that this method
        /// is not really synchronous, it's just a wrapper for the async version.
        /// </summary>
        /// <param name="id">The unique id</param>
        public void Delete(Guid id)
        {
            Task.Run(() => DeleteAsync(id)).Wait();
        }

        /// <summary>
        /// Deletes the media with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task DeleteAsync(Guid id)
        {
            var media = _db.Media
                .Include(m => m.Versions)
                .FirstOrDefault(m => m.Id == id);

            if (media != null)
            {
                using (var session = await _storage.OpenAsync())
                {
                    if (media.Versions.Count > 0)
                    {
                        foreach (var version in media.Versions)
                        {
                            // Delete version from storage
                            await session.DeleteAsync(GetResourceName(media, version.Width, version.Height, ".jpg"));
                        }
                        _db.MediaVersions.RemoveRange(media.Versions);
                    }
                    App.Hooks.OnBeforeDelete(media);

                    _db.Media.Remove(media);
                    _db.SaveChanges();

                    // Delete from storage
                    await session.DeleteAsync(media.Id + "-" + media.Filename);
                }
                App.Hooks.OnAfterDelete(media);

                RemoveFromCache(media);
            }
        }

        /// <summary>
        /// Deletes the given model. Please note that this method
        /// is not really synchronous, it's just a wrapper for the async version.
        /// </summary>
        /// <param name="model">The media</param>
        public void Delete(Media model)
        {
            Delete(model.Id);
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The media</param>
        public async Task DeleteAsync(Media model)
        {
            await DeleteAsync(model.Id);
        }

        /// <summary>
        /// Deletes the media folder with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public void DeleteFolder(Guid id)
        {
            var folder = _db.MediaFolders
                .FirstOrDefault(f => f.Id == id);

            if (folder == null)
            {
                return;
            }

            var folderCount = _db.MediaFolders.Count(f => f.ParentId == id);
            var mediaCount = _db.Media.Count(m => m.FolderId == id);

            if (folderCount != 0 || mediaCount != 0)
            {
                return;
            }

            App.Hooks.OnBeforeDelete(folder);

            _db.MediaFolders.Remove(folder);
            _db.SaveChanges();

            App.Hooks.OnAfterDelete(folder);

            RemoveFromCache(folder);
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The media</param>
        public void DeleteFolder(MediaFolder model)
        {
            DeleteFolder(model.Id);
        }

        /// <summary>
        /// Sorts the items.
        /// </summary>
        /// <param name="folders">The full folder list</param>
        /// <param name="parentId">The current parent id</param>
        /// <returns>The structure</returns>
        private MediaStructure Sort(IEnumerable<MediaFolder> folders, Guid? parentId = null, int level = 0)
        {
            var result = new MediaStructure();

            var mediaFolders = folders as MediaFolder[] ?? folders.ToArray();
            foreach (var folder in mediaFolders.Where(f => f.ParentId == parentId).OrderBy(f => f.Name))
            {
                var item = App.Mapper.Map<MediaFolder, MediaStructureItem>(folder);

                item.Level = level;
                item.Items = Sort(mediaFolders, folder.Id, level + 1);

                result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// Removes the given model from cache.
        /// </summary>
        /// <param name="model">The model</param>
        private void RemoveFromCache(Media model)
        {
            _cache?.Remove(model.Id.ToString());
        }

        /// <summary>
        /// Removes the given model from cache.
        /// </summary>
        /// <param name="model">The model</param>
        private void RemoveFromCache(MediaFolder model)
        {
            if (_cache == null)
            {
                return;
            }

            _cache.Remove(model.Id.ToString());
            RemoveStructureFromCache();
        }

        /// <summary>
        /// Removes the given model from cache.
        /// </summary>
        private void RemoveStructureFromCache()
        {
            _cache?.Remove(MEDIA_STRUCTURE);
        }

        /// <summary>
        /// Gets the media resource name.
        /// </summary>
        /// <param name="media">The media object</param>
        /// <param name="width">Optional requested width</param>
        /// <param name="height">Optional requested height</param>
        /// <param name="extension">Optional requested extension</param>
        /// <returns>The name</returns>
        private static string GetResourceName(Media media, int? width = null, int? height = null, string extension = null)
        {
            var filename = new FileInfo(media.Filename);
            var sb = new StringBuilder();

            sb.Append(media.Id);
            sb.Append("-");

            if (width.HasValue)
            {
                sb.Append(filename.Name.Replace(filename.Extension, "_"));
                sb.Append(width);

                if (height.HasValue)
                {
                    sb.Append("x");
                    sb.Append(height.Value);
                }
            }
            else
            {
                sb.Append(filename.Name.Replace(filename.Extension, ""));
            }

            sb.Append(string.IsNullOrEmpty(extension) ? filename.Extension : extension);

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
        private string GetPublicUrl(Media media, int? width = null, int? height = null, string extension = null)
        {
            var name = GetResourceName(media, width, height, extension);

            using (var config = new Config(_api))
            {
                var cdn = config.MediaCDN;

                if (!string.IsNullOrWhiteSpace(cdn))
                {
                    return cdn + name;
                }

                return _storage.GetPublicUrl(name);
            }
        }
    }
}