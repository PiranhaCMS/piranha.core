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
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Piranha.Azure
{
    /// <summary>
    /// Azure blob storage session.
    /// </summary>
    public class BlobStorageSession : IStorageSession, IDisposable
    {
        #region Members
        /// <summary>
        /// The current blob storage client.
        /// </summary>
        private readonly CloudBlobClient client;

        /// <summary>
        /// The active blob storage container.
        /// </summary>
        private readonly CloudBlobContainer container;
        #endregion

        /// <summary>
        /// Private constructor.
        /// </summary>
        /// <param name="account">The current account</param>
        /// <param name="containerName">The container name</param>
        private BlobStorageSession(CloudStorageAccount account, string containerName) {
            client = account.CreateCloudBlobClient();
            container = client.GetContainerReference(containerName);
        }

        /// <summary>
        /// Writes the content for the specified media content to the given stream.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="stream">The output stream</param>
        /// <returns>If the media was found</returns>
        public bool Get(string id, ref Stream stream) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the data for the specified media content to the given byte array.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="byte">The byte array</param>
        /// <returns>If the asset was found</returns>
        public bool Get(string id, ref byte[] bytes) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stores the given media content.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="contentType">The content type</param>
        /// <param name="stream">The input stream</param>
        /// <returns>The public URL</returns>
        public string Put(string id, string contentType, ref Stream stream) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stores the given media content.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="contentType">The content type</param>
        /// <param name="bytes">The binary data</param>
        /// <returns>The public URL</returns>
        public string Put(string id, string contentType, byte[] bytes) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the content for the specified media.
        /// </summary>
        /// <param name="id">The unique id/param>
        public bool Delete(string id) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Disposes the session.
        /// </summary>
        public void Dispose() {
            GC.SuppressFinalize(this);
        }
    }
}
