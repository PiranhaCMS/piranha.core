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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Piranha.Repositories
{
    public interface IMediaRepository
    {
        /// <summary>
        /// Gets all media available in the specified folder.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        /// <returns>The available media</returns>
        IEnumerable<Media> GetAll(Guid? folderId = null);

        /// <summary>
        /// Gets all media folders available in the specified
        /// folder.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        /// <returns>The available media folders</returns>
        IEnumerable<MediaFolder> GetAllFolders(Guid? folderId = null);

        /// <summary>
        /// Gets the media with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The media</returns>
        Media GetById(Guid id);

        /// <summary>
        /// Gets the media folder with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The media folder</returns>
        MediaFolder GetFolderById(Guid id);

        /// <summary>
        /// Gets the hierachical media structure.
        /// </summary>
        /// <returns>The media structure</returns>
        Models.MediaStructure GetStructure();

        /// <summary>
        /// Adds or updates the given model in the database depending on its state.
        /// Please note that this method is not really synchronous, it's just a 
        /// wrapper for the async version.
        /// </summary>
        /// <param name="content">The content to save</param>
        void Save(Models.MediaContent content);

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="content">The content to save</param>
        Task SaveAsync(Models.MediaContent content);

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        void SaveFolder(MediaFolder model);

        /// <summary>
        /// Moves the media to the folder with the specified id.
        /// </summary>
        /// <param name="model">The media</param>
        /// <param name="folderId">The folder id</param>
        void Move(Media model, Guid? folderId);

        /// <summary>
        /// Ensures that the image version with the given size exsists
        /// and returns its public URL. Please note that this method is 
        /// not really synchronous, it's just a wrapper for the async version.
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

        /// <summary>
        /// Deletes the media with the given id. Please note that this method
        /// is not really synchronous, it's just a wrapper for the async version.
        /// </summary>
        /// <param name="id">The unique id</param>
        void Delete(Guid id);

        /// <summary>
        /// Deletes the media with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Deletes the given model. Please note that this method
        /// is not really synchronous, it's just a wrapper for the async version.
        /// </summary>
        /// <param name="model">The media</param>
        void Delete(Media model);

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The media</param>
        Task DeleteAsync(Media model);

        /// <summary>
        /// Deletes the media folder with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        void DeleteFolder(Guid id);

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The media</param>
        void DeleteFolder(MediaFolder model);
    }
}