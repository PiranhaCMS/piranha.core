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
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Piranha.Models;
using Piranha.Repositories;

namespace Piranha.Services
{
    public class MediaService : IMediaService
    {
        private readonly IMediaRepository _repo;
        private readonly IParamService _paramService;
        private readonly IStorage _storage;
        private readonly IImageProcessor _processor;
        private readonly ICache _cache;
        private static readonly object ScaleMutex = new object();
        private const string MEDIA_STRUCTURE = "MediaStructure";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="repo">The current repository</param>
        /// <param name="paramService">The current param service</param>
        /// <param name="storage">The current storage manager</param>
        /// <param name="cache">The optional model cache</param>
        /// <param name="processor">The optional image processor</param>
        public MediaService(IMediaRepository repo, IParamService paramService, IStorage storage, IImageProcessor processor = null, ICache cache = null)
        {
            _repo = repo;
            _paramService = paramService;
            _storage = storage;
            _processor = processor;
            _cache = cache;
        }

        //Separated this into its own thing in case it needed to get reused elsewhere.
        private async Task<IEnumerable<Media>> _getFast(IEnumerable<Guid> ids)
        {
            var guids = ids as Guid[] ?? ids.ToArray();
            var partial = (_cache != null ? guids.Select(c => _cache.Get<Media>(c.ToString())) : Enumerable.Empty<Media>()).Where(c => c != null).ToArray();
            var missingIds = guids.Except(partial.Select(c => c.Id)).ToArray();
            var returns = partial.Concat((await _repo.GetById(missingIds)).Select(c =>
            {
                OnLoad(c);
                return c;
            })).OrderBy(m => m.Filename).ToArray();
            return returns;
        }

        /// <summary>
        /// Gets all media available in the specified folder.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        /// <returns>The available media</returns>
        public Task<IEnumerable<Media>> GetAllByFolderIdAsync(Guid? folderId = null)
        {
            return _repo.GetAll(folderId).ContinueWith(t => _getFast(t.Result.ToArray())).Unwrap();
        }

        /// <inheritdoc cref="IMediaService.CountFolderItemsAsync"/>
        public Task<int> CountFolderItemsAsync(Guid? folderId = null) => _repo.CountAll(folderId);

        /// <summary>
        /// Gets all media folders available in the specified
        /// folder.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        /// <returns>The available media folders</returns>
        public async Task<IEnumerable<MediaFolder>> GetAllFoldersAsync(Guid? folderId = null)
        {
            var models = new List<MediaFolder>();
            var items = await _repo.GetAllFolders(folderId).ConfigureAwait(false);

            foreach (var item in items)
            {
                var folder = await GetFolderByIdAsync(item).ConfigureAwait(false);

                if (folder != null)
                {
                    models.Add(folder);
                }
            }
            return models;
        }

        /// <summary>
        /// Gets the media with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The media</returns>
        public async Task<Media> GetByIdAsync(Guid id)
        {
            var model = _cache?.Get<Media>(id.ToString());

            if (model == null)
            {
                model = await _repo.GetById(id).ConfigureAwait(false);

                OnLoad(model);
            }
            return model;
        }

        /// <summary>
        /// Get all media matching the given IDs.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task<IEnumerable<Media>> GetByIdAsync(params Guid[] ids)
        {
            return _repo.GetById(ids);
        }

        /// <summary>
        /// Gets the media folder with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The media folder</returns>
        public async Task<MediaFolder> GetFolderByIdAsync(Guid id)
        {
            var model = _cache?.Get<MediaFolder>(id.ToString());

            if (model == null)
            {
                model = await _repo.GetFolderById(id).ConfigureAwait(false);

                OnFolderLoad(model);
            }
            return model;
        }

        /// <summary>
        /// Gets the hierachical media structure.
        /// </summary>
        /// <returns>The media structure</returns>
        public async Task<MediaStructure> GetStructureAsync()
        {
            var structure = _cache?.Get<MediaStructure>(MEDIA_STRUCTURE);

            if (structure == null)
            {
                structure = await _repo.GetStructure().ConfigureAwait(false);

                if (structure != null)
                {
                    _cache?.Set(MEDIA_STRUCTURE, structure);
                }
            }
            return structure;
        }

        /// <summary>
        /// Updates the meta data for the given media model.
        /// </summary>
        /// <param name="model">The model</param>
        public async Task SaveAsync(Media model)
        {
            // Make sure we have an existing media model with this id.
            var current = await GetByIdAsync(model.Id);

            if (current != null)
            {
                // Validate model
                var context = new ValidationContext(model);
                Validator.ValidateObject(model, context, true);

                // Call hooks & save
                App.Hooks.OnBeforeSave(model);
                await _repo.Save(model).ConfigureAwait(false);
                App.Hooks.OnAfterSave(model);

                RemoveFromCache(model);
                RemoveStructureFromCache();
            }
            else
            {
                throw new FileNotFoundException("You can only update meta data for an existing media object");
            }
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
                throw new ValidationException("Filetype not supported.");
            }

            Media model = null;

            if (content.Id.HasValue)
            {
                model = await GetByIdAsync(content.Id.Value).ConfigureAwait(false);
            }

            if (model == null)
            {
                model = new Media()
                {
                    Id = model != null || content.Id.HasValue ? content.Id.Value : Guid.NewGuid(),
                    Created = DateTime.Now
                };
                content.Id = model.Id;
            }
            else
            {
                using (var session = await _storage.OpenAsync().ConfigureAwait(false))
                {
                    // Delete all versions as we're updating the image
                    if (model.Versions.Count > 0)
                    {
                        foreach (var version in model.Versions)
                        {
                            // Delete version from storage
                            await session.DeleteAsync(GetResourceName(model, version.Width, version.Height, version.FileExtension)).ConfigureAwait(false);
                        }
                        model.Versions.Clear();
                    }

                    // Delete the old file because we might have a different filename
                    await session.DeleteAsync(GetResourceName(model)).ConfigureAwait(false);
                }
            }

            var type = App.MediaTypes.GetItem(content.Filename);

            model.Filename = content.Filename;
            model.FolderId = content.FolderId;
            model.Type = App.MediaTypes.GetMediaType(content.Filename);
            model.ContentType = type.ContentType;
            model.LastModified = DateTime.Now;

            // Pre-process if this is an image
            if (_processor != null && type.AllowProcessing && model.Type == MediaType.Image)
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

                int width, height;

                _processor.GetSize(bytes, out width, out height);
                model.Width = width;
                model.Height = height;
            }

            // Upload to storage
            using (var session = await _storage.OpenAsync().ConfigureAwait(false))
            {
                if (content is BinaryMediaContent)
                {
                    var bc = (BinaryMediaContent)content;

                    model.Size = bc.Data.Length;
                    await session.PutAsync(model.Id + "-" + model.Filename,
                        model.ContentType, bc.Data).ConfigureAwait(false);
                }
                else if (content is StreamMediaContent)
                {
                    var sc = (StreamMediaContent)content;
                    var stream = sc.Data;

                    model.Size = sc.Data.Length;
                    await session.PutAsync(model.Id + "-" + model.Filename,
                        model.ContentType, stream).ConfigureAwait(false);
                }
            }

            App.Hooks.OnBeforeSave(model);
            await _repo.Save(model).ConfigureAwait(false);
            App.Hooks.OnAfterSave(model);

            RemoveFromCache(model);
            RemoveStructureFromCache();
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        public async Task SaveFolderAsync(MediaFolder model)
        {
            // Ensure id
            if (model.Id == Guid.Empty)
            {
                model.Id = Guid.NewGuid();
            }

            // Validate model
            var context = new ValidationContext(model);
            Validator.ValidateObject(model, context, true);

            // Call hooks & save
            App.Hooks.OnBeforeSave(model);
            await _repo.SaveFolder(model).ConfigureAwait(false);
            App.Hooks.OnAfterSave(model);

            RemoveFromCache(model);
            RemoveStructureFromCache();
        }

        /// <summary>
        /// Moves the media to the folder with the specified id.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="folderId">The folder id</param>
        public async Task MoveAsync(Media model, Guid? folderId)
        {
            await _repo.Move(model, folderId).ConfigureAwait(false);
            RemoveFromCache(model);
            RemoveStructureFromCache();
        }

        /// <summary>
        /// Ensures that the image version with the given size exsists
        /// and returns its public URL.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="width">The requested width</param>
        /// <param name="height">The optionally requested height</param>
        /// <returns>The public URL</returns>
        public string EnsureVersion(Guid id, int width, int? height = null)
        {
            return EnsureVersionAsync(id, width, height).GetAwaiter().GetResult();
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
            if (_processor == null) return null;

            var media = await GetByIdAsync(id).ConfigureAwait(false);

            return media != null ? await EnsureVersionAsync(media, width, height).ConfigureAwait(false) : null;
        }

        public async Task<string> EnsureVersionAsync(Media media, int width, int? height = null)
        {
            // Get the media type
            var type = App.MediaTypes.GetItem(media.Filename);

            // If this type doesn't allow processing, return the original url
            if (!type.AllowProcessing)
                return GetPublicUrl(media);

            // If the requested size is equal to the original size, return true
            if (media.Width == width && (!height.HasValue || media.Height == height.Value))
                return GetPublicUrl(media);

            var query = media.Versions
                .Where(v => v.Width == width);

            query = height.HasValue ? query.Where(v => v.Height == height) : query.Where(v => !v.Height.HasValue);

            var version = query.FirstOrDefault();

            if (version != null)
                return media.Width == width && (!height.HasValue || media.Height == height.Value)
                    ? GetPublicUrl(media)
                    : GetPublicUrl(media, width, height, version.FileExtension);

            // Get the image file
            using (var stream = new MemoryStream())
            {
                using (var session = await _storage.OpenAsync().ConfigureAwait(false))
                {
                    if (!await session.GetAsync(media.Id + "-" + media.Filename, stream).ConfigureAwait(false))
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
                        bool upload = false;

                        lock (ScaleMutex)
                        {
                            // We have to make sure we don't scale multiple files
                            // at the same time as it can create index violations.
                            version = query.FirstOrDefault();

                            if (version == null)
                            {
                                var info = new FileInfo(media.Filename);

                                version = new MediaVersion
                                {
                                    Id = Guid.NewGuid(),
                                    Size = output.Length,
                                    Width = width,
                                    Height = height,
                                    FileExtension = info.Extension
                                };
                                media.Versions.Add(version);

                                _repo.Save(media).Wait();
                                RemoveFromCache(media);

                                upload = true;
                            }
                        }

                        if (upload)
                        {
                            return await session.PutAsync(GetResourceName(media, width, height), media.ContentType,
                                    output)
                                .ConfigureAwait(false);
                        }
                        //When moving this out of its parent method, realized that if the mutex failed, it would just fall back to the null instead of trying to return the issue.
                        //Added this to ensure that queries didn't just give up if they weren't the first to the party.
                        return GetPublicUrl(media, width, height, version.FileExtension);
                    }
                }
            }
            // If the requested size is equal to the original size, return true
        }

        /// <summary>
        /// Deletes the media with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task DeleteAsync(Guid id)
        {
            var media = await GetByIdAsync(id).ConfigureAwait(false);

            if (media != null)
            {
                using (var session = await _storage.OpenAsync().ConfigureAwait(false))
                {
                    // Delete all versions
                    if (media.Versions.Count > 0)
                    {
                        foreach (var version in media.Versions)
                        {
                            // Delete version from storage
                            await session.DeleteAsync(GetResourceName(media, version.Width, version.Height, version.FileExtension))
                                .ConfigureAwait(false);
                        }
                    }

                    // Call hooks & save
                    App.Hooks.OnBeforeDelete(media);
                    await _repo.Delete(id).ConfigureAwait(false);
                    await session.DeleteAsync(media.Id + "-" + media.Filename).ConfigureAwait(false);
                    App.Hooks.OnAfterDelete(media);
                }
                RemoveFromCache(media);
                RemoveStructureFromCache();
            }
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The media</param>
        public Task DeleteAsync(Media model)
        {
            return DeleteAsync(model.Id);
        }

        /// <summary>
        /// Deletes the media folder with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task DeleteFolderAsync(Guid id)
        {
            var folder = await GetFolderByIdAsync(id).ConfigureAwait(false);

            if (folder != null)
            {
                //
                // TODO: Check empty
                //
                // var folderCount = _db.MediaFolders.Count(f => f.ParentId == id);
                // var mediaCount = _db.Media.Count(m => m.FolderId == id);

                // if (folderCount == 0 && mediaCount == 0)
                // {

                // Call hooks & delete
                App.Hooks.OnBeforeDelete(folder);
                await _repo.DeleteFolder(id).ConfigureAwait(false);
                App.Hooks.OnAfterDelete(folder);

                RemoveFromCache(folder);
                //}
            }
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The media</param>
        public Task DeleteFolderAsync(MediaFolder model)
        {
            return DeleteFolderAsync(model.Id);
        }

        /// <summary>
        /// Processes the model on load.
        /// </summary>
        /// <param name="model">The model</param>
        private void OnLoad(Media model)
        {
            if (model != null)
            {
                // Get public url
                model.PublicUrl = GetPublicUrl(model);

                // Create missing properties
                foreach (var key in App.MediaTypes.MetaProperties)
                {
                    if (!model.Properties.Any(p => p.Key == key))
                    {
                        model.Properties.Add(key, null);
                    }
                }

                App.Hooks.OnLoad(model);

                _cache?.Set(model.Id.ToString(), model);
            }
        }

        /// <summary>
        /// Processes the model on load.
        /// </summary>
        /// <param name="model">The model</param>
        private void OnFolderLoad(MediaFolder model)
        {
            if (model != null)
            {
                App.Hooks.OnLoad(model);

                _cache?.Set(model.Id.ToString(), model);
            }
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
            if (_cache != null)
            {
                _cache.Remove(model.Id.ToString());
                RemoveStructureFromCache();
            }
        }

        /// <summary>
        /// Removes the media structure from cache.
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
        private string GetResourceName(Media media, int? width = null, int? height = null, string extension = null)
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

            if (string.IsNullOrEmpty(extension))
            {
                sb.Append(filename.Extension);
            }
            else
            {
                sb.Append(extension);
            }
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

            using (var config = new Config(_paramService))
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