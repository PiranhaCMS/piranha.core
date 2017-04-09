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
            if (folderId == null) {
                return db.Query<Media>($"SELECT * FROM [{TABLE}] WHERE [FolderId] IS NULL ORDER BY [Filename]",
                transaction: transaction);
            } else {
                return db.Query<Media>($"SELECT * FROM [{TABLE}] WHERE [FolderId]=@FolderId ORDER BY [Filename]", new {
                    FolderId = folderId
                }, transaction: transaction);
            }
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
            return db.QuerySingleOrDefault<Media>($"SELECT * FROM [{TABLE}] WHERE [Id]=@Id", new { 
                Id = id 
            }, transaction: transaction);
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
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="data">The binary data</param>
        /// <param name="transaction">The optional transaction</param>
        public void Save(Models.MediaContent content, IDbTransaction transaction = null) {
            var media = GetById(content.Id, transaction);
            var insert = false;

            if (media == null) {
                media = new Media() {
                    Id = !string.IsNullOrWhiteSpace(content.Id) ? content.Id : Guid.NewGuid().ToString()
                };
                insert = true;
            }
            media.Filename = content.Filename;
            media.FolderId = content.FolderId;
            media.ContentType = content.ContentType;

            // Upload to storage
            using (var session = storage.Open()) {
                if (content is Models.BinaryMediaContent) {
                    var bc = (Models.BinaryMediaContent)content;

                    media.Size = bc.Data.Length;
                    media.PublicUrl = session.Put(media.Id + "-" + media.Filename, 
                        media.ContentType, bc.Data);
                } else if (content is Models.StreamMediaContent) {
                    var sc = (Models.StreamMediaContent)content;
                    var stream = sc.Data;

                    media.Size = sc.Data.Length;
                    media.PublicUrl = session.Put(media.Id + "-" + media.Filename, 
                        media.ContentType, ref stream);
                }
            }

            if (insert)
                db.Execute($"INSERT INTO [{TABLE}] ([Id],[FolderId],[Filename],[ContentType],[Size],[PublicUrl],[Created],[LastModified]) VALUES(@Id,@FolderId,@Filename,@ContentType,@Size,@PublicUrl,@Created,@LastModified)",
                    media, transaction: transaction);
            else db.Execute($"UPDATE [{TABLE}] SET [Id]=@Id,[FolderId]=@FolderId,[Filename]=@Filename,[ContentType]=@ContentType,[Size]=@Size,[PublicUrl]=@PublicUrl,[Created]=@Created,[LastModified]=@LastModified",
                media, transaction: transaction);
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="transaction">The optional transaction</param>
        public void SaveFolder(MediaFolder model, IDbTransaction transaction = null) {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The media</param>
        /// <param name="transaction">The optional transaction</param>
        public void DeleteFolder(MediaFolder model, IDbTransaction transaction = null) {
            throw new NotImplementedException();
        }
    }
}