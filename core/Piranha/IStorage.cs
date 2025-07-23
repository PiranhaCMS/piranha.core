/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Models;

namespace Piranha;

/// <summary>
/// Service for handling asset storage.
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
    /// <param name="media">The media file</param>
    /// <param name="filename">The file name</param>
    /// <returns>The public url</returns>
    string GetPublicUrl(Media media, string filename);

    /// <summary>
    /// Gets the resource name for the given media object.
    /// </summary>
    /// <param name="media">The media file</param>
    /// <param name="filename">The file name</param>
    /// <returns>The public url</returns>
    string GetResourceName(Media media, string filename);
}
