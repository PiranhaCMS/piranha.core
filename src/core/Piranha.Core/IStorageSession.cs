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
using System.IO;

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
		bool Get(string id, ref Stream stream);

		/// <summary>
		/// Writes the data for the specified media content to the given byte array.
		/// </summary>
		/// <param name="id">The unique id</param>
		/// <param name="byte">The byte array</param>
		/// <returns>If the asset was found</returns>
		bool Get(string id, ref byte[] bytes);

		/// <summary>
		/// Stores the given media content.
		/// </summary>
		/// <param name="id">The unique id</param>
		/// <param name="contentType">The content type</param>
		/// <param name="stream">The input stream</param>
		/// <returns>The public URL</returns>
		string Put(string id, string contentType, ref Stream stream);

		/// <summary>
		/// Stores the given media content.
		/// </summary>
		/// <param name="id">The unique id</param>
		/// <param name="contentType">The content type</param>
		/// <param name="bytes">The binary data</param>
		/// <returns>The public URL</returns>
		string Put(string id, string contentType, byte[] bytes);

		/// <summary>
		/// Deletes the content for the specified media.
		/// </summary>
		/// <param name="id">The unique id/param>
		bool Delete(string id);
    }
}
