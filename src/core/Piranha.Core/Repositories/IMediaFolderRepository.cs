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
    public interface IMediaFolderRepository
    {
        /// <summary>
        /// Gets the folder with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The media folder</returns>
        MediaFolder GetById(Guid id);

        /// <summary>
        /// Gets the available folders.
        /// </summary>
        /// <param name="parentId">The optional parent folder id</param>
        /// <returns>The available folders</returns>
        IList<MediaFolder> Get(Guid? parentId = null);

        /// <summary>
        /// Moves the folder with the given id to the
        /// specified parent.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="parentId">The parent folder id</param>
        void Move(Guid id, Guid? parentId);

        /// <summary>
        /// Saves the given media folder.
        /// </summary>
        /// <param name="folder">The media folder</param>
        void Save(MediaFolder folder);

        /// <summary>
        /// Deletes the given media folder
        /// </summary>
        /// <param name="folder">The media folder</param>
        void Delete(MediaFolder folder);

        /// <summary>
        /// Deletes the media folder with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        void Delete(Guid id);        
    }
}
