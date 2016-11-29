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
        /// Disposes the current session.
        /// </summary>
        public void Dispose() {
            GC.SuppressFinalize(this);
        }
    }
}
