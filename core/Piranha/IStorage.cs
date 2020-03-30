/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Threading.Tasks;

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
        Task<IStorageSession> OpenAsync();

        /// <summary>
        /// Gets the public URL for the given media object.
        /// </summary>
        /// <param name="id">The file id</param>
        /// <returns>The public url</returns>
        string GetPublicUrl(string id);
    }
}
