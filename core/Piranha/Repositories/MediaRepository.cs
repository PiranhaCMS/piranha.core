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

namespace Piranha.Repositories
{
    public class MediaRepository : IMediaRepository
    {
        private readonly IDbConnection db;
        private readonly IStorage storage;
        private const string TABLE = "Piranha_Media";
        private const string FOLDERTABLE = "Piranha_MediaFolders";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="connection">The current connection</param>
        /// <param name="storage">The current storage manager</param>
        public MediaRepository(IDbConnection connection, IStorage storage) {
            this.db = connection;
            this.storage = storage;
        }

        /// <summary>
        /// Gets all media available in the specified folder.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        /// <param name="transaction">The optional transaction</param>
        /// <returns>The available media</returns>
        public IEnumerable<Media> GetAll(string folderId = null, IDbTransaction transaction = null) {
            IEnumerable<Media> models;

            if (folderId == null) {
                models = db.Query<Media>($"SELECT * FROM [{TABLE}] WHERE [FolderId] IS NULL ORDER BY [Filename]",
                    transaction: transaction);
            } else {
                models = db.Query<Media>($"SELECT * FROM [{TABLE}] WHERE [FolderId]=@FolderId ORDER BY [Filename]", new {
                    FolderId = folderId
                }, transaction: transaction);
            }

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
        /// <param name="transaction">The optional transaction</param>
        /// <returns>The available media folders</returns>
        public IEnumerable<MediaFolder> GetAllFolders(string folderId = null, IDbTransaction transaction = null) {
            if (folderId == null) {
                return db.Query<MediaFolder>($"SELECT * FROM [{FOLDERTABLE}] WHERE [ParentId] IS NULL ORDER BY [Name]",
                transaction: transaction);
            } else {
                return db.Query<MediaFolder>($"SELECT * FROM [{FOLDERTABLE}] WHERE [ParentId]=@FolderId ORDER BY [Name]", new {
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
        public Media GetById(string id, IDbTransaction transaction = null) {
            var model = db.QuerySingleOrDefault<Media>($"SELECT * FROM [{TABLE}] WHERE [Id]=@Id", new { 
                Id = id 
            }, transaction: transaction);

            if (model != null) {
                model.PublicUrl = storage.GetPublicUrl(model);
            }
            return model;
        }

        /// <summary>
        /// Gets the media folder with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="transaction">The optional transaction</param>
        /// <returns>The media folder</returns>
        public MediaFolder GetFolderById(string id, IDbTransaction transaction = null) {
            return db.QuerySingleOrDefault<MediaFolder>($"SELECT * FROM [{FOLDERTABLE}] WHERE [Id]=@Id", new { 
                Id = id 
            }, transaction: transaction);
        }

        /// <summary>
        /// Gets the hierachical media structure.
        /// </summary>
        /// <param name="transaction">The optional transaction</param>
        /// <returns>The media structure</returns>
        public Models.MediaStructure GetStructure(IDbTransaction transaction = null) {
            var folders = db.Query<MediaFolder>("SELECT [Id],[ParentId],[Name],[Created] FROM [Piranha_MediaFolders] ORDER BY [ParentId], [Name]",
                transaction: transaction);

            return Sort(folders);
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="data">The binary data</param>
        /// <param name="transaction">The optional transaction</param>
        public void Save(Models.MediaContent content, IDbTransaction transaction = null) {
            if (!App.MediaTypes.IsSupported(content.Filename))
                throw new NotSupportedException("Filetype not supported.");

            var model = GetById(content.Id, transaction);
            var insert = false;

            if (model == null) {
                model = new Media() {
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

            if (insert) {
                // Prepare
                model.Created = DateTime.Now;
                model.LastModified = DateTime.Now;

                db.Execute($"INSERT INTO [{TABLE}] ([Id],[FolderId],[Type],[Filename],[ContentType],[Size],[Created],[LastModified]) VALUES(@Id,@FolderId,@Type,@Filename,@ContentType,@Size,@Created,@LastModified)",
                    model, transaction: transaction);
            } else {
                model.LastModified = DateTime.Now;

                db.Execute($"UPDATE [{TABLE}] SET [Id]=@Id,[FolderId]=@FolderId,[Type]=@Type,[Filename]=@Filename,[ContentType]=@ContentType,[Size]=@Size,[LastModified]=@LastModified WHERE [Id]=@Id",
                    model, transaction: transaction);
            }
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="transaction">The optional transaction</param>
        public void SaveFolder(Data.MediaFolder model, IDbTransaction transaction = null) {
            var insert = db.ExecuteScalar<int>($"SELECT COUNT(*) FROM [{FOLDERTABLE}] where [Id]=@Id", model, transaction: transaction) == 0;

            // Prepare
            model.Id = !string.IsNullOrWhiteSpace(model.Id) ? model.Id : Guid.NewGuid().ToString();
            model.Created = insert ? DateTime.Now : model.Created;

            if (insert) {
                db.Execute($"INSERT INTO [{FOLDERTABLE}] ([Id],[ParentId],[Name],[Created]) VALUES(@Id,@ParentId,@Name,@Created)",
                    model, transaction: transaction);
            } else {
                db.Execute($"UPDATE [{FOLDERTABLE}] SET [Id]=@Id,[ParentId]=@ParentId,[Name]=@Name WHERE [Id]=@Id",
                    model, transaction: transaction);
            }        
        }

        /// <summary>
        /// Deletes the media with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="transaction">The optional transaction</param>
        public void Delete(string id, IDbTransaction transaction = null) {
            var media = GetById(id, transaction);

            if (media != null)
                Delete(media);
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The media</param>
        /// <param name="transaction">The optional transaction</param>
        public void Delete(Media model, IDbTransaction transaction = null) {
            db.Execute($"DELETE FROM [{TABLE}] WHERE [Id]=@Id", model, transaction: transaction);

            // Delete from storage
            using (var session = storage.Open()) {
                session.Delete(model.Id + "-" + model.Filename);
            }
        }

        /// <summary>
        /// Deletes the media folder with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="transaction">The optional transaction</param>
        public void DeleteFolder(string id, IDbTransaction transaction = null) {
            db.Execute($"DELETE FROM [{FOLDERTABLE}] WHERE [Id]=@Id", new { Id = id }, transaction: transaction);
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The media</param>
        /// <param name="transaction">The optional transaction</param>
        public void DeleteFolder(Data.MediaFolder model, IDbTransaction transaction = null) {
            DeleteFolder(model.Id, transaction);
        }

        /// <summary>
        /// Sorts the items.
        /// </summary>
        /// <param name="folders">The full folder list</param>
        /// <param name="parentId">The current parent id</param>
        /// <returns>The structure</returns>
        private Models.MediaStructure Sort(IEnumerable<MediaFolder> folders, string parentId = null, int level = 0) {
            var result = new Models.MediaStructure();

            foreach (var folder in folders.Where(f => f.ParentId == parentId).OrderBy(f => f.Name)) {
                var item = App.Mapper.Map<MediaFolder, Models.MediaStructureItem>(folder);

                item.Level = level;
                item.Items = Sort(folders, folder.Id, level + 1);

                result.Add(item);
            }
            return result;
        }
    }
}