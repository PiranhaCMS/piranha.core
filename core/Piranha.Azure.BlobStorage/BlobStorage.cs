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
        private readonly CloudStorageAccount _storage;

        /// <summary>
        /// The name of the container to use.
        /// </summary>
        private readonly string _containerName;

        /// <summary>
        /// The container url.
        /// </summary>
        private string _containerUrl;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BlobStorage(StorageCredentials credentials, string containerName = "uploads")
        {
            _storage = new CloudStorageAccount(credentials, true);
            _containerName = containerName;
        }

        /// <summary>
        /// Opens a new storage session.
        /// </summary>
        /// <returns>A new open session</returns>
        public async Task<IStorageSession> OpenAsync()
        {
            var session = _storage.CreateCloudBlobClient();
            var container = session.GetContainerReference(_containerName);

            if (!await container.ExistsAsync())
            {
                await container.CreateAsync();
                await container.SetPermissionsAsync(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });
            }
            _containerUrl = container.Uri.AbsoluteUri;

            return new BlobStorageSession(container);
        }

        /// <summary>
        /// Gets the public URL for the given media object.
        /// </summary>
        /// <param name="id">The media resource id</param>
        /// <returns>The public url</returns>
        public string GetPublicUrl(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            if (!string.IsNullOrEmpty(_containerUrl))
            {
                return _containerUrl + "/" + id;
            }

            var session = _storage.CreateCloudBlobClient();
            var container = session.GetContainerReference(_containerName);

            _containerUrl = container.Uri.AbsoluteUri;
            return _containerUrl + "/" + id;
        }
    }
}
