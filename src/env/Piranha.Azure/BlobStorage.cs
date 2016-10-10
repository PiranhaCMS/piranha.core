/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Threading.Tasks;
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
		public async Task<IStorageSession> OpenAsync() {
			return await BlobStorageSession.Open(account, configuration);
		}
    }
}
