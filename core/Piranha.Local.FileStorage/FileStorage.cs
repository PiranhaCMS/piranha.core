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
using System.Threading.Tasks;

namespace Piranha.Local
{
    public class FileStorage : IStorage
    {
        private readonly string basePath = "wwwroot/uploads/";
        private readonly string baseUrl = "~/uploads/";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="basePath">The optional base path</param>
        /// <param name="basePath">The optional base url</param>
        public FileStorage(string basePath = null, string baseUrl = null) {
            if (!string.IsNullOrEmpty(basePath)) {
                this.basePath = basePath;
                this.baseUrl = baseUrl;
            }

            if (!Directory.Exists(this.basePath))
                Directory.CreateDirectory(this.basePath);
        }

        /// <summary>
        /// Opens a new storage session.
        /// </summary>
        /// <returns>A new open session</returns>
        public Task<IStorageSession> OpenAsync() {
            return Task.Run(() => {
                return (IStorageSession)new FileStorageSession(basePath, baseUrl);
            });
        }

        /// <summary>
        /// Gets the public URL for the given media object.
        /// </summary>
        /// <param name="media">The media</param>
        /// <returns>The public url</returns>
        public string GetPublicUrl(Data.Media media) {
            if (media.Id != Guid.Empty)
                return baseUrl + media.Id + "-" + media.Filename;
            return null;
        }
    }
}
