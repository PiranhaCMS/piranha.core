/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using System.Threading.Tasks;

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
        /// Creates a new Blog Storage service from the given credentials.
        /// </summary>
        public BlobStorage(StorageCredentials credentials, string containerName = "uploads")
        {
            _storage = new CloudStorageAccount(credentials, true);
            _containerName = containerName;
        }

        /// <summary>
        /// Creates a new Blob Storage service from the given connection string.
        /// </summary>
        public BlobStorage(string connectionString, string containerName = "uploads")
        {
            _storage = CloudStorageAccount.Parse(connectionString);
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
                await container.SetPermissionsAsync(new BlobContainerPermissions()
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
            if (!string.IsNullOrWhiteSpace(id))
            {
                if (string.IsNullOrEmpty(_containerUrl))
                {
                    var session = _storage.CreateCloudBlobClient();
                    var container = session.GetContainerReference(_containerName);

                    _containerUrl = container.Uri.AbsoluteUri;
                }
                return _containerUrl + "/" + id;
            }
            return null;
        }
    }
}
