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
    public interface IMediaRepository
    {
        /// <summary>
        /// Gets the media item with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The media item</returns>
        MediaItem GetById(Guid id);

        /// <summary>
        /// Gets the full media model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The media model</returns>
        Media GetModelById(Guid id);

        /// <summary>
        /// Gets the available media items.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        /// <returns>The media items</returns>
        IList<MediaItem> Get(Guid? folderId = null);

        /// <summary>
        /// Saves the media content.
        /// </summary>
        /// <param name="content">The media content</param>
        void Save(MediaContent content);

        /// <summary>
        /// Deletes the given media item.
        /// </summary>
        /// <param name="media">The media item</param>
        void Delete(MediaItem media);
        
        /// <summary>
        /// Deletes the given media model.
        /// </summary>
        /// <param name="media">The media model</param>
        void Delete(Media media);
        
        /// <summary>
        /// Deletes the media with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        void Delete(Guid id);        
    }
}
