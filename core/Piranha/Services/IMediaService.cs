/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Piranha.Models;

namespace Piranha.Services
{
    public interface IMediaService
    {
        /// <summary>
        /// Gets all media available in the specified folder.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        /// <returns>The available media</returns>
        Task<IEnumerable<Media>> GetAllByFolderIdAsync(Guid? folderId = null);

        /// <summary>
        /// Get the amount of media available in the specified folder.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        /// <returns>The quantity of media items present.</returns>
        Task<int> CountFolderItemsAsync(Guid? folderId = null);

        /// <summary>
        /// Gets all media folders available in the specified
        /// folder.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        /// <returns>The available media folders</returns>
        Task<IEnumerable<MediaFolder>> GetAllFoldersAsync(Guid? folderId = null);

        /// <summary>
        /// Gets the media with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The media</returns>
        Task<Media> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets the media with the given id.
        /// </summary>
        /// <param name="ids">The unique id</param>
        /// <returns>The media</returns>
        Task<IEnumerable<Media>> GetByIdAsync(params Guid[] ids);

        /// <summary>
        /// Gets the media folder with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The media folder</returns>
        Task<MediaFolder> GetFolderByIdAsync(Guid id);

        /// <summary>
        /// Gets the hierachical media structure.
        /// </summary>
        /// <returns>The media structure</returns>
        Task<MediaStructure> GetStructureAsync();

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="content">The content to save</param>
        Task SaveAsync(MediaContent content);

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        Task SaveFolderAsync(MediaFolder model);

        /// <summary>
        /// Moves the media to the folder with the specified id.
        /// </summary>
        /// <param name="media">The media</param>
        /// <param name="folderId">The folder id</param>
        Task MoveAsync(Media model, Guid? folderId);

        /// <summary>
        /// Ensures that the image version with the given size exsists
        /// and returns its public URL.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="width">The requested width</param>
        /// <param name="height">The optionally requested height</param>
        /// <returns>The public URL</returns>
        string EnsureVersion(Guid id, int width, int? height = null);

        /// <summary>
        /// Ensures that the image version with the given size exsists
        /// and returns its public URL.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="width">The requested width</param>
        /// <param name="height">The optionally requested height</param>
        /// <returns>The public URL</returns>
        Task<string> EnsureVersionAsync(Guid id, int width, int? height = null);

        Task<string> EnsureVersionAsync(Media media, int width, int? height = null);

        /// <summary>
        /// Deletes the media with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The media</param>
        Task DeleteAsync(Media model);

        /// <summary>
        /// Deletes the media folder with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        Task DeleteFolderAsync(Guid id);

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The media</param>
        Task DeleteFolderAsync(MediaFolder model);
    }
}