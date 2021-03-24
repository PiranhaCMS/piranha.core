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
using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Piranha.Models;

namespace Piranha.Azure
{
    public class BlobStorage : IStorage, IStorageSession
    {
        /// <summary>
        /// The private storage account.
        /// </summary>
        private readonly BlobContainerClient _storage;

        /// <summary>
        /// How uploaded files should be named to
        /// ensure uniqueness.
        /// </summary>
        private readonly BlobStorageNaming _naming;

        /// <summary>
        /// Creates a new Blog Storage service from the given credentials.
        /// </summary>
        /// <param name="blobContainerUri">
        /// A <see cref="Uri"/> referencing the blob service.
        /// This is likely to be similar to "https://{account_name}.blob.core.windows.net".
        /// </param>
        /// <param name="credentials">The connection credentials</param>
        /// <param name="naming">How uploaded media files should be named</param>
        public BlobStorage(
            Uri blobContainerUri,
            TokenCredential credentials,
            BlobStorageNaming naming = BlobStorageNaming.UniqueFileNames)
        {
            _storage = new BlobContainerClient(blobContainerUri, credentials);
            _naming = naming;
        }

        /// <summary>
        /// Creates a new Blob Storage service from the given connection string.
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        /// <param name="containerName">The container name</param>
        /// <param name="naming">How uploaded media files should be named</param>
        public BlobStorage(
            string connectionString,
            string containerName = "uploads",
            BlobStorageNaming naming = BlobStorageNaming.UniqueFileNames)
        {
            _storage = new BlobContainerClient(connectionString, containerName);
            _naming = naming;
        }

        /// <summary>
        /// Opens a new storage session.
        /// </summary>
        /// <returns>A new open session</returns>
        public async Task<IStorageSession> OpenAsync()
        {
            if (!await _storage.ExistsAsync())
            {
                await _storage.CreateAsync(PublicAccessType.Blob);
            }

            return this;
        }

        /// <summary>
        /// Gets the public URL for the given media object.
        /// </summary>
        /// <param name="media">The media file</param>
        /// <param name="id">The resource id</param>
        /// <returns>The public url</returns>
        public string GetPublicUrl(Media media, string id)
        {
            if (media != null && !string.IsNullOrWhiteSpace(id))
            {
                return $"{ _storage.Uri.AbsoluteUri }/{ GetResourceName(media, id, true) }";
            }
            return null;
        }

        /// <summary>
        /// Gets the resource name for the given media object.
        /// </summary>
        /// <param name="media">The media file</param>
        /// <param name="filename">The file name</param>
        /// <returns>The public url</returns>
        public string GetResourceName(Media media, string filename)
        {
            return GetResourceName(media, filename, false);
        }

        /// <summary>
        /// Gets the resource name for the given media object.
        /// </summary>
        /// <param name="media">The media file</param>
        /// <param name="filename">The file name</param>
        /// <param name="encode">If the filename should be URL encoded</param>
        /// <returns>The public url</returns>
        public string GetResourceName(Media media, string filename, bool encode)
        {
            if (_naming == BlobStorageNaming.UniqueFileNames)
            {
                return $"{ media.Id }-{ (encode ? System.Web.HttpUtility.UrlPathEncode(filename) : filename) }";
            }
            else
            {
                return $"{ media.Id }/{ (encode ? System.Web.HttpUtility.UrlPathEncode(filename) : filename) }";
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the content for the specified media content to the given stream.
        /// </summary>
        /// <param name="media">The media file</param>
        /// <param name="filename">The file name</param>
        /// <param name="stream">The output stream</param>
        /// <returns>If the media was found</returns>
        public async Task<bool> GetAsync(Media media, string filename, Stream stream)
        {
            var blob = _storage.GetBlobClient(GetResourceName(media, filename));

            if (await blob.ExistsAsync())
            {
                await blob.DownloadToAsync(stream);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Stores the given media content.
        /// </summary>
        /// <param name="media">The media file</param>
        /// <param name="filename">The file name</param>
        /// <param name="contentType">The content type</param>
        /// <param name="stream">The input stream</param>
        /// <returns>The public URL</returns>
        public async Task<string> PutAsync(Media media, string filename, string contentType, Stream stream)
        {
            var blob = _storage.GetBlobClient(GetResourceName(media, filename));

            var blobHttpHeader = new BlobHttpHeaders {ContentType = contentType};

            await blob.UploadAsync(stream, blobHttpHeader);

            return blob.Uri.AbsoluteUri;
        }

        /// <summary>
        /// Stores the given media content.
        /// </summary>
        /// <param name="media">The media file</param>
        /// <param name="filename">The file name</param>
        /// <param name="contentType">The content type</param>
        /// <param name="bytes">The binary data</param>
        /// <returns>The public URL</returns>
        public async Task<string> PutAsync(Media media, string filename, string contentType, byte[] bytes)
        {
            using (var ms = new MemoryStream())
            {
                var writer = new StreamWriter(ms);
                writer.Write(bytes);
                writer.Flush();
                ms.Position = 0;
                return await PutAsync(media, filename, contentType, ms);
            }
        }

        /// <summary>
        /// Deletes the content for the specified media.
        /// </summary>
        /// <param name="media">The media file</param>
        /// <param name="filename">The file name</param>
        public async Task<bool> DeleteAsync(Media media, string filename)
        {
            var blob = _storage.GetBlobClient(GetResourceName(media, filename));

            return await blob.DeleteIfExistsAsync();
        }
    }
}
