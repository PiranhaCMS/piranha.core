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

namespace Piranha.Local.Storage
{
    public class FileStorageSession : IStorageSession
    {
		#region Members
		private const string MEDIA_PATH = "wwwroot/uploads/";
		private const string MEDIA_URL = "~/uploads/";
		#endregion

		/// <summary>
		/// Writes the content for the specified media content to the given stream.
		/// </summary>
		/// <param name="id">The unique id</param>
		/// <param name="stream">The output stream</param>
		/// <returns>If the media was found</returns>
		public bool Get(string id, ref Stream stream) {
			if (File.Exists(MEDIA_PATH + id)) {
				using (var file = File.OpenRead(MEDIA_PATH + id)) {
					file.CopyTo(stream);
					return true;
				}
			}
			return false;
        }

		/// <summary>
		/// Writes the data for the specified media content to the given byte array.
		/// </summary>
		/// <param name="id">The unique id</param>
		/// <param name="byte">The byte array</param>
		/// <returns>If the asset was found</returns>
		public bool Get(string id, ref byte[] bytes) {
			if (File.Exists(MEDIA_PATH + id)) {
				using (var file = File.OpenRead(MEDIA_PATH + id)) {
					bytes = new byte[file.Length];
					return file.Read(bytes, 0, (int)file.Length) > 0;
				}
			}
			return false;
        }

		/// <summary>
		/// Stores the given media content.
		/// </summary>
		/// <param name="id">The unique id</param>
		/// <param name="contentType">The content type</param>
		/// <param name="stream">The input stream</param>
		/// <returns>The public URL</returns>
		public string Put(string id, string contentType, ref Stream stream) {
			using (var file = File.OpenWrite(MEDIA_PATH + id)) {
				stream.CopyTo(file);
			}
			return MEDIA_URL + id;
        }

		/// <summary>
		/// Stores the given media content.
		/// </summary>
		/// <param name="id">The unique id</param>
		/// <param name="contentType">The content type</param>
		/// <param name="bytes">The binary data</param>
		/// <returns>The public URL</returns>
		public string Put(string id, string contentType, byte[] bytes) {
			using (var file = File.OpenWrite(MEDIA_PATH + id)) {
				file.Write(bytes, 0, bytes.Length);
			}
			return MEDIA_URL + id;
        }

		/// <summary>
		/// Deletes the content for the specified media.
		/// </summary>
		/// <param name="id">The unique id/param>
		public bool Delete(string id) {
			if (File.Exists(MEDIA_PATH + id)) {
				File.Delete(MEDIA_PATH + id);
				return true;
			}
			return false;
        }

        /// <summary>
        /// Disposes the session.
        /// </summary>
        public void Dispose() {
            GC.SuppressFinalize(this);
        }
    }
}
