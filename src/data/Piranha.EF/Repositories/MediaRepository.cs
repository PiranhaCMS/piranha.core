/*
 * Copyright (c) 2016 Håkan Edling
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
using Piranha.Repositories;

namespace Piranha.EF.Repositories
{
    public class MediaRepository : RepositoryBase<Data.Media, Models.MediaItem>, IMediaRepository
    {
        #region Members
        private readonly IStorage storage;
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        internal MediaRepository(IDb db, IStorage storage) : base(db) { 
            this.storage = storage;
        }

        /// <summary>
        /// Gets the full media model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The media model</returns>
        public Models.Media GetModelById(Guid id) {
            var media = Query().FirstOrDefault(m => m.Id == id);

            if (media != null)
                return Module.Mapper.Map<Data.Media, Models.Media>(media);
            return null;
        }

        /// <summary>
        /// Gets the available media items.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        /// <returns>The media items</returns>
        public IList<Models.MediaItem> Get(Guid? folderId = null) {
            var items = Query().Where(m => m.FolderId == folderId).ToList();
            var result = new List<Models.MediaItem>();

            foreach (var item in items)
                result.Add(Map(item));
            return result;
        }

        /// <summary>
        /// Saves the media content.
        /// </summary>
        /// <param name="content">The media content</param>
        public void Save(Models.MediaContent content) {
            var media = Query().FirstOrDefault(m => m.Id == content.Id);

            if (media == null) {
                media = new Data.Media() {
                    Id = content.Id.HasValue ? content.Id.Value : Guid.NewGuid()
                };
                db.Media.Add(media);
            }

            media.Type = content.Type;
            media.FileName = content.Filename;
            media.FolderId = content.FolderId;
            media.ContentType = content.ContentType;
            media.Description = content.Description;

            // Upload to storage
            using (var session = storage.Open()) {
                if (content is Models.BinaryMediaContent) {
                    var bc = (Models.BinaryMediaContent)content;

                    media.FileSize = bc.Data.Length;
                    media.PublicUrl = session.Put(media.Id.ToString() + "-" + media.FileName, 
                        media.ContentType, bc.Data);
                } else if (content is Models.StreamMediaContent) {
                    var sc = (Models.StreamMediaContent)content;
                    var stream = sc.Data;

                    media.FileSize = sc.Data.Length;
                    media.PublicUrl = session.Put(media.Id.ToString() + "-" + media.FileName, 
                        media.ContentType, ref stream);
                }

            }
            db.SaveChanges();
        }

        /// <summary>
        /// Moves the media with the given id to the 
        /// specified folder.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="folderId">The folder id</param>
        public void Move(Guid id, Guid? folderId) {
            var media = Query().FirstOrDefault(m => m.Id == id);

            if (media != null) {
                media.FolderId = folderId;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes the given media item.
        /// </summary>
        /// <param name="media">The media item</param>
        public void Delete(Models.MediaItem media) {
            Delete(media.Id);
        }
        
        /// <summary>
        /// Deletes the given media model.
        /// </summary>
        /// <param name="media">The media model</param>
        public void Delete(Models.Media media) {
            Delete(media.Id);
        }
        
        /// <summary>
        /// Deletes the media with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public void Delete(Guid id) {
            var media = Query().FirstOrDefault(m => m.Id == id);

            if (media != null) {
                // Delete from storage
                using (var session = storage.Open()) {
                    session.Delete(id.ToString() + "-" + media.FileName);
                }
                db.Media.Remove(media);
                db.SaveChanges();
            }
        }        
    }
}
