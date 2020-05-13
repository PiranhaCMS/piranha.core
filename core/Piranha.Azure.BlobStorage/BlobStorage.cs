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
using Piranha.Models;

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
        /// How uploaded files should be named to
        /// ensure uniqueness.
        /// </summary>
        private readonly BlobStorageNaming _naming;

        /// <summary>
        /// Creates a new Blog Storage service from the given credentials.
        /// </summary>
        /// <param name="credentials">The connection credentials</param>
        /// <param name="containerName">The container name</param>
        /// <param name="naming">How uploaded media files should be named</param>
        public BlobStorage(
            StorageCredentials credentials,
            string containerName = "uploads",
            BlobStorageNaming naming = BlobStorageNaming.UniqueFileNames)
        {
            _storage = new CloudStorageAccount(credentials, true);
            _containerName = containerName;
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
            _storage = CloudStorageAccount.Parse(connectionString);
            _containerName = containerName;
            _naming = naming;
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

            return new BlobStorageSession(this, container, _naming);
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
                if (string.IsNullOrEmpty(_containerUrl))
                {
                    var session = _storage.CreateCloudBlobClient();
                    var container = session.GetContainerReference(_containerName);

                    _containerUrl = container.Uri.AbsoluteUri;
                }
                return $"{ _containerUrl }/{ GetResourceName(media, id) }";
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
            if (_naming == BlobStorageNaming.UniqueFileNames)
            {
                return $"{ media.Id }-{ filename }";
            }
            else
            {
                return $"{ media.Id }/{ filename }";
            }
        }
    }
}
