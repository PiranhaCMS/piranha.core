/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha
{
    /// <summary>
    /// Interface for the main storage manager.
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        /// Opens a new storage session.
        /// </summary>
        /// <returns>A new open session</returns>
        IStorageSession Open();

        /// <summary>
        /// Gets the public URL for the given media object.
        /// </summary>
        /// <param name="media">The media</param>
        /// <returns>The public url</returns>
        string GetPublicUrl(Data.Media media);
    }
}
