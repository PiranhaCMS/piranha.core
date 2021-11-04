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
using Piranha.Models;

namespace Piranha.Local
{
    public class FileStorage : IStorage
    {
        private readonly string _basePath = "wwwroot/uploads/";
        private readonly string _baseUrl = "~/uploads/";
        private readonly FileStorageNaming _naming;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="basePath">The optional base path</param>
        /// <param name="baseUrl">The optional base url</param>
        /// <param name="naming">How uploaded media files should be named</param>
        public FileStorage(
            string basePath = null,
            string baseUrl = null,
            FileStorageNaming naming = FileStorageNaming.UniqueFileNames)
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

            _naming = naming;
        }

        /// <summary>
        /// Opens a new storage session.
        /// </summary>
        /// <returns>A new open session</returns>
        public Task<IStorageSession> OpenAsync()
        {
            return Task.Run(() =>
            {
                return (IStorageSession)new FileStorageSession(this, _basePath, _baseUrl, _naming);
            });
        }

        /// <summary>
        /// Gets the public URL for the given media object.
        /// </summary>
        /// <param name="media">The media file</param>
        /// <param name="filename">The file name</param>
        /// <returns>The public url</returns>
        public string GetPublicUrl(Media media, string filename)
        {
            if (media != null && !string.IsNullOrWhiteSpace(filename))
            {
                return _baseUrl + GetResourceName(media, filename, true);
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
            return GetResourceName(media, filename, false);
        }

        /// <summary>
        /// Gets the resource name for the given media object.
        /// </summary>
        /// <param name="media">The media file</param>
        /// <param name="filename">The file name</param>
        /// <param name="encode">If the filename should be URL encoded</param>
        /// <returns>The public url</returns>
        public string GetResourceName(Media media, string filename, bool encode)
        {
            if (media != null && !string.IsNullOrWhiteSpace(filename))
            {
                var path = "";

                if (_naming == FileStorageNaming.UniqueFileNames)
                {
                    path = $"{ media.Id }-{ (encode ? System.Web.HttpUtility.UrlPathEncode(filename) : filename) }";
                }
                else
                {
                    path = $"{ media.Id }/{ (encode ? System.Web.HttpUtility.UrlPathEncode(filename) : filename) }";
                }
                return path;
            }
            return null;
        }
    }
}
