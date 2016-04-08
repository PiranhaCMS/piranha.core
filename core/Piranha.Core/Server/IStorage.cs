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

namespace Piranha.Server
{
	/// <summary>
	/// Interface for handling uploaded files.
	/// </summary>
	public interface IStorage : IDisposable
	{
		/// <summary>
		/// Gets the binary data for the file with the given
		/// name from the storage.
		/// </summary>
		/// <param name="name">The unique name</param>
		/// <param name="stream">The output stream</param>
		/// <returns>If the file was found and could be opened</returns>
		bool Get(string name, ref Stream stream);

		/// <summary>
		/// Puts the binary data available in the given stream in
		/// storage with the given name and content type.
		/// </summary>
		/// <param name="name">The unique name</param>
		/// <param name="contentType">The content type</param>
		/// <param name="stream">The input stream</param>
		/// <returns>The public URL for the file</returns>
		string Put(string name, string contentType, ref Stream stream);

		/// <summary>
		/// Deletes specified file from storage.
		/// </summary>
		/// <param name="name">The unique name</param>
		bool Delete(string name);
	}
}
