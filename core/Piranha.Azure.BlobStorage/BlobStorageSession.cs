/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.Azure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;
using Piranha.Models;

namespace Piranha.Azure
{
    public class BlobStorageSession : IStorageSession
    {
        /// <summary>
        /// The main storage service;
        /// </summary>
        private readonly BlobStorage _storage;

        /// <summary>
        /// The container in which to store media.
        /// </summary>
        private readonly CloudBlobContainer _container;

        /// <summary>
        /// The current naming convention.
        /// </summary>
        private readonly BlobStorageNaming _naming;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="storage">The current storage</param>
        /// <param name="container">The current container</param>
        /// <param name="naming">How uploaded media files should be named</param>
        public BlobStorageSession(BlobStorage storage, CloudBlobContainer container, BlobStorageNaming naming)
        {
            _storage = storage;
            _container = container;
            _naming = naming;
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
            var blob = _container.GetBlockBlobReference(_storage.GetResourceName(media, filename));

            if (await blob.ExistsAsync())
            {
                await blob.DownloadToStreamAsync(stream);
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
            var blob = _container.GetBlockBlobReference(_storage.GetResourceName(media, filename));

            await blob.UploadFromStreamAsync(stream);
            blob.Properties.ContentType = contentType;
            await blob.SetPropertiesAsync();

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
            var blob = _container.GetBlockBlobReference(_storage.GetResourceName(media, filename));

            await blob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);
            blob.Properties.ContentType = contentType;
            await blob.SetPropertiesAsync();

            return blob.Uri.AbsoluteUri;
        }

        /// <summary>
        /// Deletes the content for the specified media.
        /// </summary>
        /// <param name="media">The media file</param>
        /// <param name="filename">The file name</param>
        public async Task<bool> DeleteAsync(Media media, string filename)
        {
            var blob = _container.GetBlockBlobReference(_storage.GetResourceName(media, filename));

            if (await blob.ExistsAsync())
            {
                await blob.DeleteAsync();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Disposes the session.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
