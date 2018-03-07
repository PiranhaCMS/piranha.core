/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Piranha.Azure
{
    public class BlobStorage : IStorage
    {
        /// <summary>
        /// The private storage account.
        /// </summary>
        private readonly CloudStorageAccount storage;

        /// <summary>
        /// The name of the container to use.
        /// </summary>
        private readonly string containerName;

        /// <summary>
        /// The container url.
        /// </summary>
        private string containerUrl;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BlobStorage(StorageCredentials credentials, string containerName = "uploads") {
            storage = new CloudStorageAccount(credentials, true);
            this.containerName = containerName;
        }

        /// <summary>
        /// Opens a new storage session.
        /// </summary>
        /// <returns>A new open session</returns>
        public async Task<IStorageSession> OpenAsync() {
            var session = storage.CreateCloudBlobClient();
            var container = session.GetContainerReference(containerName);

            if (!await container.ExistsAsync()) {
                await container.CreateAsync();
                await container.SetPermissionsAsync(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });
            }
            containerUrl = container.Uri.AbsoluteUri;

            return new BlobStorageSession(container);
        }

        /// <summary>
        /// Gets the public URL for the given media object.
        /// </summary>
        /// <param name="id">The media resource id</param>
        /// <returns>The public url</returns>
        public string GetPublicUrl(string id) {
            if (!string.IsNullOrWhiteSpace(id)) {
                if (string.IsNullOrEmpty(containerUrl)) {
                    var session = storage.CreateCloudBlobClient();
                    var container = session.GetContainerReference(containerName);

                    containerUrl = container.Uri.AbsoluteUri;                
                }
                return containerUrl + "/" + id;
            }
            return null;
        }
    }
}
