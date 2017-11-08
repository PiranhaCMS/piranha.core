/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Piranha.Data;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Piranha.Repositories.Async
{
    public interface IMediaRepository
    {
        /// <summary>
        /// Gets all media available in the specified folder.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        /// <param name="transaction">The optional transaction</param>
        /// <returns>The available media</returns>
        Task<IEnumerable<Media>> GetAll(string folderId = null, IDbTransaction transaction = null);

        /// <summary>
        /// Gets all media folders available in the specified
        /// folder.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        /// <param name="transaction">The optional transaction</param>
        /// <returns>The available media folders</returns>
       Task<IEnumerable<MediaFolder>> GetAllFolders(string folderId = null, IDbTransaction transaction = null);

        /// <summary>
        /// Gets the media with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="transaction">The optional transaction</param>
        /// <returns>The media</returns>
       Task<Media> GetById(string id, IDbTransaction transaction = null);

        /// <summary>
        /// Gets the media folder with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="transaction">The optional transaction</param>
        /// <returns>The media folder</returns>
        Task<MediaFolder> GetFolderById(string id, IDbTransaction transaction = null);

        /// <summary>
        /// Gets the hierachical media structure.
        /// </summary>
        /// <param name="transaction">The optional transaction</param>
        /// <returns>The media structure</returns>
        Task<Models.MediaStructure> GetStructure(IDbTransaction transaction = null);

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="data">The binary data</param>
        /// <param name="transaction">The optional transaction</param>
        Task Save(Models.MediaContent content, IDbTransaction transaction = null);

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="transaction">The optional transaction</param>
        Task SaveFolder(MediaFolder model, IDbTransaction transaction = null);

        /// <summary>
        /// Deletes the media with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="transaction">The optional transaction</param>
        Task Delete(string id, IDbTransaction transaction = null);

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The media</param>
        /// <param name="transaction">The optional transaction</param>
        Task Delete(Media model, IDbTransaction transaction = null);

        /// <summary>
        /// Deletes the media folder with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="transaction">The optional transaction</param>
        Task DeleteFolder(string id, IDbTransaction transaction = null);

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The media</param>
        /// <param name="transaction">The optional transaction</param>
        Task DeleteFolder(MediaFolder model, IDbTransaction transaction = null);
    }
}