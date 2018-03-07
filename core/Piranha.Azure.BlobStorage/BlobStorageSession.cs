/*
 * Copyright (c) 2018 Håkan Edling
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
using Microsoft.WindowsAzure.Storage.Blob;

namespace Piranha.Azure
{
    public class BlobStorageSession : IStorageSession
    {
		/// <summary>
		/// The container in which to store media.
		/// </summary>
		private CloudBlobContainer container;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public BlobStorageSession(CloudBlobContainer container) {
			this.container = container;
		}

		/// <summary>
		/// Writes the content for the specified media content to the given stream.
		/// </summary>
		/// <param name="id">The unique id</param>
		/// <param name="stream">The output stream</param>
		/// <returns>If the media was found</returns>
		public async Task<bool> GetAsync(string id, Stream stream) {
			var blob = container.GetBlockBlobReference(id);

			if (await blob.ExistsAsync()) {
				await blob.DownloadToStreamAsync(stream);
				return true;
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
		public async Task<string> PutAsync(string id, string contentType, Stream stream) {
			var blob = container.GetBlockBlobReference(id);

			await blob.UploadFromStreamAsync(stream);
			blob.Properties.ContentType = contentType;
			await blob.SetPropertiesAsync();

			return blob.Uri.AbsoluteUri;
        }

		/// <summary>
		/// Stores the given media content.
		/// </summary>
		/// <param name="id">The unique id</param>
		/// <param name="contentType">The content type</param>
		/// <param name="bytes">The binary data</param>
		/// <returns>The public URL</returns>
		public async Task<string> PutAsync(string id, string contentType, byte[] bytes) {
			var blob = container.GetBlockBlobReference(id);

			await blob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);
			blob.Properties.ContentType = contentType;
			await blob.SetPropertiesAsync();

			return blob.Uri.AbsoluteUri;
        }

		/// <summary>
		/// Deletes the content for the specified media.
		/// </summary>
		/// <param name="id">The unique id/param>
		public async Task<bool> DeleteAsync(string id) {
			var blob = container.GetBlockBlobReference(id);

			if (await blob.ExistsAsync()) {
				await blob.DeleteAsync();
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
