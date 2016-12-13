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
using Piranha.Models;

namespace Piranha.Repositories
{
    public class MediaRepository : IMediaRepository
    {
        /// <summary>
        /// The private data service.
        /// </summary>
        private readonly IDataService service;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="service">The current data service</param>
        public MediaRepository(IDataService service) {
            this.service = service;
        }

        /// <summary>
        /// Gets the media item with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The media item</returns>
        public MediaItem GetById(Guid id) {
            return service.Media.GetById(id);
        }

        /// <summary>
        /// Gets the full media model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The media model</returns>
        public Media GetModelById(Guid id) {
            return service.Media.GetModelById(id);
        }

        /// <summary>
        /// Gets the available media items.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        /// <returns>The media items</returns>
        public IList<MediaItem> Get(Guid? folderId = null) {
            return service.Media.Get(folderId);
        }

        /// <summary>
        /// Saves the media content.
        /// </summary>
        /// <param name="content">The media content</param>
        public void Save(MediaContent content) {
            if (content is BinaryMediaContent || content is StreamMediaContent) {
                // Check that the given file is supported
                if (App.MediaTypes.IsSupported(content.Filename)) {
                    // Set the media type
                    content.Type = App.MediaTypes.GetMediaType(content.Filename);
                    service.Media.Save(content);
                } else {
                    throw new NotSupportedException("The given file type is not supported.");
                }
            }
        }

        /// <summary>
        /// Moves the media with the given id to the 
        /// specified folder.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="folderId">The folder id</param>
        public void Move(Guid id, Guid? folderId) {
            service.Media.Move(id, folderId);
        }

        /// <summary>
        /// Deletes the given media item.
        /// </summary>
        /// <param name="media">The media item</param>
        public void Delete(MediaItem media) {
            service.Media.Delete(media);
        }
        
        /// <summary>
        /// Deletes the given media model.
        /// </summary>
        /// <param name="media">The media model</param>
        public void Delete(Media media) {
            service.Media.Delete(media);
        }
        
        /// <summary>
        /// Deletes the media with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public void Delete(Guid id) {
            service.Media.Delete(id);
        }
    }
}
