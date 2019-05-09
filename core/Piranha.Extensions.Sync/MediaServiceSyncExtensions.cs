/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Piranha.Models;

namespace Piranha.Services
{
    public static class MediaServiceSyncExtensions
    {
        /// <summary>
        /// Gets all media available in the specified folder.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        /// <returns>The available media</returns>
        public static IEnumerable<Media> GetAll(this IMediaService service, Guid? folderId = null)
        {
            return service.GetAllAsync(folderId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets all media folders available in the specified
        /// folder.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        /// <returns>The available media folders</returns>
        public static IEnumerable<MediaFolder> GetAllFolders(this IMediaService service, Guid? folderId = null)
        {
            return service.GetAllFoldersAsync(folderId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the media with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The media</returns>
        public static Media GetById(this IMediaService service, Guid id)
        {
            return service.GetByIdAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the media folder with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The media folder</returns>
        public static MediaFolder GetFolderById(this IMediaService service, Guid id)
        {
            return service.GetFolderByIdAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the hierachical media structure.
        /// </summary>
        /// <returns>The media structure</returns>
        public static Models.MediaStructure GetStructure(this IMediaService service)
        {
            return service.GetStructureAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="content">The content to save</param>
        public static void Save(this IMediaService service, Models.MediaContent content)
        {
            service.SaveAsync(content).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        public static void SaveFolder(this IMediaService service, MediaFolder model)
        {
            service.SaveFolderAsync(model).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Moves the media to the folder with the specified id.
        /// </summary>
        /// <param name="media">The media</param>
        /// <param name="folderId">The folder id</param>
        public static void Move(this IMediaService service, Media model, Guid? folderId)
        {
            service.MoveAsync(model, folderId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Deletes the media with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public static void Delete(this IMediaService service, Guid id)
        {
            service.DeleteAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The media</param>
        public static void Delete(this IMediaService service, Media model)
        {
            service.DeleteAsync(model).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Deletes the media folder with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public static void DeleteFolder(this IMediaService service, Guid id)
        {
            service.DeleteFolderAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The media</param>
        public static void DeleteFolder(this IMediaService service, MediaFolder model)
        {
            service.DeleteFolderAsync(model).GetAwaiter().GetResult();
        }
    }
}
