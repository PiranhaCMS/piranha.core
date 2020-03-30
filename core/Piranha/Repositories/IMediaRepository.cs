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
using System.Threading.Tasks;
using Piranha.Models;

namespace Piranha.Repositories
{
    public interface IMediaRepository
    {
        /// <summary>
        /// Gets all media available in the specified folder.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        /// <returns>The available media</returns>
        Task<IEnumerable<Guid>> GetAll(Guid? folderId = null);

        /// <summary>
        /// Count the amount of items in the given folder.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        /// <returns></returns>
        Task<int> CountAll(Guid? folderId);

        /// <summary>
        /// Gets all media folders available in the specified
        /// folder.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        /// <returns>The available media folders</returns>
        Task<IEnumerable<Guid>> GetAllFolders(Guid? folderId = null);

        /// <summary>
        /// Get media for all Ids in this enumerable.
        /// </summary>
        /// <param name="ids">One or several media id</param>
        /// <returns>The matching media</returns>
        Task<IEnumerable<Media>> GetById(params Guid[] ids);

        /// <summary>
        /// Gets the media with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The media</returns>
        Task<Media> GetById(Guid id);

        /// <summary>
        /// Gets the media folder with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The media folder</returns>
        Task<MediaFolder> GetFolderById(Guid id);

        /// <summary>
        /// Gets the hierachical media structure.
        /// </summary>
        /// <returns>The media structure</returns>
        Task<MediaStructure> GetStructure();

        /// <summary>
        /// Adds or updates the given model in the database.
        /// </summary>
        /// <param name="model">The model</param>
        Task Save(Media model);

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        Task SaveFolder(MediaFolder model);

        /// <summary>
        /// Moves the media to the folder with the specified id.
        /// </summary>
        /// <param name="model">The media</param>
        /// <param name="folderId">The folder id</param>
        Task Move(Media model, Guid? folderId);

        /// <summary>
        /// Deletes the media with the given id. Please note that this method
        /// is not really synchronous, it's just a wrapper for the async version.
        /// </summary>
        /// <param name="id">The unique id</param>
        Task Delete(Guid id);

        /// <summary>
        /// Deletes the media folder with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        Task DeleteFolder(Guid id);
    }
}