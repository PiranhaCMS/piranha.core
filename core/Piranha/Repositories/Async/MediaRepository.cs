/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Dapper;
using Piranha.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Piranha.Repositories.Async
{
    public class MediaRepository : IMediaRepository
    {
        private readonly IDbConnection db;
        private readonly IStorage storage;
        private const string TABLE = "Piranha_Media";
        private const string FOLDERTABLE = "Piranha_MediaFolders";
        private readonly ICache cache;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="connection">The current connection</param>
        /// <param name="storage">The current storage manager</param>
        /// <param name="cache">The optional model cache</param>
        public MediaRepository(IDbConnection connection, IStorage storage, ICache cache = null)
        {
            this.db = connection;
            this.storage = storage;
            this.cache = cache;
        }

        /// <summary>
        /// Gets all media available in the specified folder.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        /// <param name="transaction">The optional transaction</param>
        /// <returns>The available media</returns>
        public async Task<IEnumerable<Media>> GetAll(string folderId = null, IDbTransaction transaction = null)
        {
            IEnumerable<Media> models;

            if (folderId == null)
            {
                models = await db.QueryAsync<Media>($"SELECT * FROM [{TABLE}] WHERE [FolderId] IS NULL ORDER BY [Filename]",
                    transaction: transaction);
            }
            else
            {
                models = await db.QueryAsync<Media>($"SELECT * FROM [{TABLE}] WHERE [FolderId]=@FolderId ORDER BY [Filename]", new
                {
                    FolderId = folderId
                }, transaction: transaction);
            }

            foreach (var model in models)
            {
                model.PublicUrl = storage.GetPublicUrl(model);
            }
            return models;
        }

        /// <summary>
        /// Gets all media folders available in the specified
        /// folder.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        /// <param name="transaction">The optional transaction</param>
        /// <returns>The available media folders</returns>
        public async Task<IEnumerable<MediaFolder>> GetAllFolders(string folderId = null, IDbTransaction transaction = null)
        {
            if (folderId == null)
            {
                return await db.QueryAsync<MediaFolder>($"SELECT * FROM [{FOLDERTABLE}] WHERE [ParentId] IS NULL ORDER BY [Name]",
                transaction: transaction);
            }
            else
            {
                return await db.QueryAsync<MediaFolder>($"SELECT * FROM [{FOLDERTABLE}] WHERE [ParentId]=@FolderId ORDER BY [Name]", new
                {
                    FolderId = folderId
                }, transaction: transaction);
            }
        }

        /// <summary>
        /// Gets the media with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="transaction">The optional transaction</param>
        /// <returns>The media</returns>
        public async Task<Media> GetById(string id, IDbTransaction transaction = null)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                Media model = cache != null ? cache.Get<Media>(id) : null;

                if (model == null)
                {
                    model = await db.QuerySingleOrDefaultAsync<Media>($"SELECT * FROM [{TABLE}] WHERE [Id]=@Id", new
                    {
                        Id = id
                    }, transaction: transaction);

                    if (model != null)
                        model.PublicUrl = storage.GetPublicUrl(model);

                    if (cache != null && model != null)
                        cache.Set(model.Id, model);
                }
                return model;
            }
            return null;
        }

        /// <summary>
        /// Gets the media folder with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="transaction">The optional transaction</param>
        /// <returns>The media folder</returns>
        public async Task<MediaFolder> GetFolderById(string id, IDbTransaction transaction = null)
        {
            MediaFolder model = cache != null ? cache.Get<MediaFolder>(id) : null;

            if (model == null)
            {
                model = await db.QuerySingleOrDefaultAsync<MediaFolder>($"SELECT * FROM [{FOLDERTABLE}] WHERE [Id]=@Id", new
                {
                    Id = id
                }, transaction: transaction);

                if (cache != null && model != null)
                    cache.Set(model.Id, model);
            }
            return model;
        }

        /// <summary>
        /// Gets the hierachical media structure.
        /// </summary>
        /// <param name="transaction">The optional transaction</param>
        /// <returns>The media structure</returns>
        public async Task<Models.MediaStructure> GetStructure(IDbTransaction transaction = null)
        {
            Models.MediaStructure model = cache != null ? cache.Get<Models.MediaStructure>("MediaStructure") : null;

            if (model == null)
            {
                var folders = await db.QueryAsync<MediaFolder>("SELECT [Id],[ParentId],[Name],[Created] FROM [Piranha_MediaFolders] ORDER BY [ParentId], [Name]",
                    transaction: transaction);

                model = await Sort(folders);

                if (cache != null && model != null)
                    cache.Set("MediaStructure", model);
            }
            return model;
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="data">The binary data</param>
        /// <param name="transaction">The optional transaction</param>
        public async Task Save(Models.MediaContent content, IDbTransaction transaction = null)
        {
            if (!App.MediaTypes.IsSupported(content.Filename))
                throw new NotSupportedException("Filetype not supported.");

            var model = await GetById(content.Id, transaction);
            var insert = false;

            if (model == null)
            {
                model = new Media()
                {
                    Id = model != null || !string.IsNullOrWhiteSpace(content.Id) ? content.Id : Guid.NewGuid().ToString()
                };
                content.Id = model.Id;
                insert = true;
            }

            model.Filename = content.Filename;
            model.FolderId = content.FolderId;
            model.Type = App.MediaTypes.GetMediaType(content.Filename);
            model.ContentType = App.MediaTypes.GetContentType(content.Filename);

            // Upload to storage
            using (var session = storage.Open())
            {
                if (content is Models.BinaryMediaContent)
                {
                    var bc = (Models.BinaryMediaContent)content;

                    model.Size = bc.Data.Length;
                    session.Put(model.Id + "-" + model.Filename,
                        model.ContentType, bc.Data);
                }
                else if (content is Models.StreamMediaContent)
                {
                    var sc = (Models.StreamMediaContent)content;
                    var stream = sc.Data;

                    model.Size = sc.Data.Length;
                    session.Put(model.Id + "-" + model.Filename,
                        model.ContentType, ref stream);
                }
            }

            if (insert)
            {
                // Prepare
                model.Created = DateTime.Now;
                model.LastModified = DateTime.Now;

                await db.ExecuteAsync($"INSERT INTO [{TABLE}] ([Id],[FolderId],[Type],[Filename],[ContentType],[Size],[Created],[LastModified]) VALUES(@Id,@FolderId,@Type,@Filename,@ContentType,@Size,@Created,@LastModified)",
                     model, transaction: transaction);
            }
            else
            {
                model.LastModified = DateTime.Now;

                await db.ExecuteAsync($"UPDATE [{TABLE}] SET [Id]=@Id,[FolderId]=@FolderId,[Type]=@Type,[Filename]=@Filename,[ContentType]=@ContentType,[Size]=@Size,[LastModified]=@LastModified WHERE [Id]=@Id",
                     model, transaction: transaction);

                RemoveFromCache(model);
            }
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="transaction">The optional transaction</param>
        public async Task SaveFolder(Data.MediaFolder model, IDbTransaction transaction = null)
        {
            var insert = await db.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM [{FOLDERTABLE}] where [Id]=@Id", model, transaction: transaction) == 0;

            // Prepare
            model.Id = !string.IsNullOrWhiteSpace(model.Id) ? model.Id : Guid.NewGuid().ToString();
            model.Created = insert ? DateTime.Now : model.Created;

            if (insert)
            {
                await db.ExecuteAsync($"INSERT INTO [{FOLDERTABLE}] ([Id],[ParentId],[Name],[Created]) VALUES(@Id,@ParentId,@Name,@Created)",
                     model, transaction: transaction);
            }
            else
            {
                await db.ExecuteAsync($"UPDATE [{FOLDERTABLE}] SET [Id]=@Id,[ParentId]=@ParentId,[Name]=@Name WHERE [Id]=@Id",
                     model, transaction: transaction);
            }
            RemoveStructureFromCache();
        }

        /// <summary>
        /// Deletes the media with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="transaction">The optional transaction</param>
        public async Task Delete(string id, IDbTransaction transaction = null)
        {
            var media = await GetById(id, transaction);

            if (media != null)
                await Delete(media);
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The media</param>
        /// <param name="transaction">The optional transaction</param>
        public async Task Delete(Media model, IDbTransaction transaction = null)
        {
            await db.ExecuteAsync($"DELETE FROM [{TABLE}] WHERE [Id]=@Id", model, transaction: transaction);

            // Delete from storage
            using (var session = storage.Open())
            {
                session.Delete(model.Id + "-" + model.Filename);
            }
            RemoveFromCache(model);
        }

        /// <summary>
        /// Deletes the media folder with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="transaction">The optional transaction</param>
        public async Task DeleteFolder(string id, IDbTransaction transaction = null)
        {
            var model = await GetFolderById(id, transaction);

            if (model != null)
                await DeleteFolder(model, transaction);
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The media</param>
        /// <param name="transaction">The optional transaction</param>
        public async Task DeleteFolder(Data.MediaFolder model, IDbTransaction transaction = null)
        {
            await db.ExecuteAsync($"DELETE FROM [{FOLDERTABLE}] WHERE [Id]=@Id", model, transaction: transaction);
            RemoveFromCache(model);
        }

        /// <summary>
        /// Sorts the items.
        /// </summary>
        /// <param name="folders">The full folder list</param>
        /// <param name="parentId">The current parent id</param>
        /// <returns>The structure</returns>
        private async Task<Models.MediaStructure> Sort(IEnumerable<MediaFolder> folders, string parentId = null, int level = 0)
        {
            var result = new Models.MediaStructure();

            foreach (var folder in folders.Where(f => f.ParentId == parentId).OrderBy(f => f.Name))
            {
                var item = App.Mapper.Map<MediaFolder, Models.MediaStructureItem>(folder);

                item.Level = level;
                item.Items = await Sort(folders, folder.Id, level + 1);

                result.Add(item);
            }
            return result;
        }

        /// <summary>
        /// Removes the given model from cache.
        /// </summary>
        /// <param name="model">The model</param>
        private void RemoveFromCache(Data.Media model)
        {
            if (cache != null)
                cache.Remove(model.Id.ToString());
        }

        /// <summary>
        /// Removes the given model from cache.
        /// </summary>
        /// <param name="model">The model</param>
        private void RemoveFromCache(MediaFolder model)
        {
            if (cache != null)
            {
                cache.Remove(model.Id.ToString());
                RemoveStructureFromCache();
            }
        }

        /// <summary>
        /// Removes the given model from cache.
        /// </summary>
        /// <param name="model">The model</param>
        private void RemoveStructureFromCache()
        {
            if (cache != null)
                cache.Remove("MediaStructure");
        }
    }
}