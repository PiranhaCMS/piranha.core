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

namespace Piranha.Local
{
    public class FileStorageSession : IStorageSession
    {
        #region Members
        private readonly string _basePath;
        private readonly string _baseUrl;
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="basePath">The base path</param>
        /// <param name="baseUrl">The base url</param>
        public FileStorageSession(string basePath, string baseUrl)
        {
            _basePath = basePath;
            _baseUrl = baseUrl;
        }

        /// <summary>
        /// Writes the content for the specified media content to the given stream.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="stream">The output stream</param>
        /// <returns>If the media was found</returns>
        public async Task<bool> GetAsync(string id, Stream stream)
        {
            if (File.Exists(_basePath + id))
            {
                using (var file = File.OpenRead(_basePath + id))
                {
                    await file.CopyToAsync(stream);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Stores the given media content.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="contentType">The content type</param>
        /// <param name="stream">The input stream</param>
        /// <returns>The public URL</returns>
        public async Task<string> PutAsync(string id, string contentType, Stream stream)
        {
            using (var file = File.OpenWrite(_basePath + id))
            {
                await stream.CopyToAsync(file).ConfigureAwait(false);
            }
            return _baseUrl + id;
        }

        /// <summary>
        /// Stores the given media content.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="contentType">The content type</param>
        /// <param name="bytes">The binary data</param>
        /// <returns>The public URL</returns>
        public async Task<string> PutAsync(string id, string contentType, byte[] bytes)
        {
            using (var file = File.OpenWrite(_basePath + id))
            {
                await file.WriteAsync(bytes, 0, bytes.Length);
            }
            return _baseUrl + id;
        }

        /// <summary>
        /// Deletes the content for the specified media.
        /// </summary>
        /// <param name="id">The unique id</param>
        public Task<bool> DeleteAsync(string id)
        {
            return Task.Run(() =>
            {
                if (File.Exists(_basePath + id))
                {
                    File.Delete(_basePath + id);
                    return true;
                }
                return false;
            });
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
