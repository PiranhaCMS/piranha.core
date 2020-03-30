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
using System.IO;
using System.Threading.Tasks;

namespace Piranha
{
    /// <summary>
    /// Interface for a storage session.
    /// </summary>
    public interface IStorageSession : IDisposable
    {
		/// <summary>
		/// Writes the content for the specified media content to the given stream.
		/// </summary>
		/// <param name="id">The unique id</param>
		/// <param name="stream">The output stream</param>
		/// <returns>If the media was found</returns>
		Task<bool> GetAsync(string id, Stream stream);

		/// <summary>
		/// Stores the given media content.
		/// </summary>
		/// <param name="id">The unique id</param>
		/// <param name="contentType">The content type</param>
		/// <param name="stream">The input stream</param>
		/// <returns>The public URL</returns>
		Task<string> PutAsync(string id, string contentType, Stream stream);

		/// <summary>
		/// Stores the given media content.
		/// </summary>
		/// <param name="id">The unique id</param>
		/// <param name="contentType">The content type</param>
		/// <param name="bytes">The binary data</param>
		/// <returns>The public URL</returns>
		Task<string> PutAsync(string id, string contentType, byte[] bytes);

		/// <summary>
		/// Deletes the content for the specified media.
		/// </summary>
		/// <param name="id">The unique id</param>
		Task<bool> DeleteAsync(string id);
    }
}
