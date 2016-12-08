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
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;

namespace Piranha.Azure
{
    /// <summary>
    /// Azure blob storage manager.
    /// </summary>
    public class BlobStorage : IStorage
    {
        #region Members
        private readonly CloudStorageAccount account;
        private readonly IConfigurationRoot configuration;
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="config">The curren configuraiton manager</param>
        public BlobStorage(IConfigurationRoot config) {
            account = CloudStorageAccount.Parse(config["Piranha:AzureStorage"]);
            configuration = config;
        }

        /// <summary>
        /// Opens a new storage session.
        /// </summary>
        /// <returns>A new open session</returns>
        public IStorageSession Open() {
            throw new NotImplementedException();
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
        
    }
}
