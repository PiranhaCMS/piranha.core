/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.IO;
using System.Threading.Tasks;

namespace Piranha.Local
{
    public class FileStorage : IStorage
    {
        private readonly string _basePath = "wwwroot/uploads/";
        private readonly string _baseUrl = "~/uploads/";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="basePath">The optional base path</param>
        /// <param name="baseUrl">The optional base url</param>
        public FileStorage(string basePath = null, string baseUrl = null)
        {
            if (!string.IsNullOrEmpty(basePath))
            {
                _basePath = basePath;
            }
            if (!string.IsNullOrEmpty(baseUrl))
            {
                _baseUrl = baseUrl;
            }

            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }
        }

        /// <summary>
        /// Opens a new storage session.
        /// </summary>
        /// <returns>A new open session</returns>
        public Task<IStorageSession> OpenAsync()
        {
            return Task.Run(() =>
            {
                return (IStorageSession)new FileStorageSession(_basePath, _baseUrl);
            });
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
                return _baseUrl + id;
            }
            return null;
        }
    }
}
