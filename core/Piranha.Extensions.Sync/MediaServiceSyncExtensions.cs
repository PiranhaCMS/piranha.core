/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.Collections.Generic;
using Piranha.Models;

namespace Piranha.Services
{
    public static class MediaServiceSyncExtensions
    {
        /// <summary>
        /// Gets all media available in the specified folder.
        /// </summary>
        /// <param name="service">The media service</param>
        /// <param name="folderId">The optional folder id</param>
        /// <returns>The available media</returns>
        [Obsolete]
        public static IEnumerable<Media> GetAll(this IMediaService service, Guid? folderId = null)
        {
            return service.GetAllByFolderIdAsync(folderId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets all media folders available in the specified
        /// folder.
        /// </summary>
        /// <param name="service">The media service</param>
        /// <param name="folderId">The optional folder id</param>
        /// <returns>The available media folders</returns>
        [Obsolete]
        public static IEnumerable<MediaFolder> GetAllFolders(this IMediaService service, Guid? folderId = null)
        {
            return service.GetAllFoldersAsync(folderId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the media with the given id.
        /// </summary>
        /// <param name="service">The media service</param>
        /// <param name="id">The unique id</param>
        /// <returns>The media</returns>
        [Obsolete]
        public static Media GetById(this IMediaService service, Guid id)
        {
            return service.GetByIdAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the media folder with the given id.
        /// </summary>
        /// <param name="service">The media service</param>
        /// <param name="id">The unique id</param>
        /// <returns>The media folder</returns>
        [Obsolete]
        public static MediaFolder GetFolderById(this IMediaService service, Guid id)
        {
            return service.GetFolderByIdAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the hierachical media structure.
        /// </summary>
        /// <param name="service">The media service</param>
        /// <returns>The media structure</returns>
        [Obsolete]
        public static Models.MediaStructure GetStructure(this IMediaService service)
        {
            return service.GetStructureAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="service">The media service</param>
        /// <param name="content">The content to save</param>
        [Obsolete]
        public static void Save(this IMediaService service, Models.MediaContent content)
        {
            service.SaveAsync(content).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="service">The media service</param>
        /// <param name="model">The model</param>
        [Obsolete]
        public static void SaveFolder(this IMediaService service, MediaFolder model)
        {
            service.SaveFolderAsync(model).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Moves the media to the folder with the specified id.
        /// </summary>
        /// <param name="service">The media service</param>
        /// <param name="model">The media</param>
        /// <param name="folderId">The folder id</param>
        [Obsolete]
        public static void Move(this IMediaService service, Media model, Guid? folderId)
        {
            service.MoveAsync(model, folderId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Deletes the media with the given id.
        /// </summary>
        /// <param name="service">The media service</param>
        /// <param name="id">The unique id</param>
        [Obsolete]
        public static void Delete(this IMediaService service, Guid id)
        {
            service.DeleteAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="service">The media service</param>
        /// <param name="model">The media</param>
        [Obsolete]
        public static void Delete(this IMediaService service, Media model)
        {
            service.DeleteAsync(model).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Deletes the media folder with the given id.
        /// </summary>
        /// <param name="service">The media service</param>
        /// <param name="id">The unique id</param>
        [Obsolete]
        public static void DeleteFolder(this IMediaService service, Guid id)
        {
            service.DeleteFolderAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="service">The media service</param>
        /// <param name="model">The media</param>
        [Obsolete]
        public static void DeleteFolder(this IMediaService service, MediaFolder model)
        {
            service.DeleteFolderAsync(model).GetAwaiter().GetResult();
        }
    }
}
