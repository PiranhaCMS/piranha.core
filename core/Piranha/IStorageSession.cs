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
/// Interface for a storage session.
/// </summary>
public interface IStorageSession : IDisposable
{
	/// <summary>
	/// Writes the content for the specified media content to the given stream.
	/// </summary>
	/// <param name="media">The media file</param>
	/// <param name="filename">The file name</param>
	/// <param name="stream">The output stream</param>
	/// <returns>If the media was found</returns>
	Task<bool> GetAsync(Media media, string filename, Stream stream);

	/// <summary>
	/// Stores the given media content.
	/// </summary>
	/// <param name="media">The media file</param>
	/// <param name="filename">The file name</param>
	/// <param name="contentType">The content type</param>
	/// <param name="stream">The input stream</param>
	/// <returns>The public URL</returns>
	Task<string> PutAsync(Media media, string filename, string contentType, Stream stream);

	/// <summary>
	/// Stores the given media content.
	/// </summary>
	/// <param name="media">The media file</param>
	/// <param name="filename">The file name</param>
	/// <param name="contentType">The content type</param>
	/// <param name="bytes">The binary data</param>
	/// <returns>The public URL</returns>
	Task<string> PutAsync(Media media, string filename, string contentType, byte[] bytes);

	/// <summary>
	/// Deletes the content for the specified media.
	/// </summary>
	/// <param name="media">The media file</param>
	/// <param name="filename">The file name</param>
	Task<bool> DeleteAsync(Media media, string filename);
}
