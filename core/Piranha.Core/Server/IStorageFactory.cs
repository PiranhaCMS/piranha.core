/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha.Server
{
	/// <summary>
	/// Interface for creating storage sessions.
	/// </summary>
	public interface IStorageFactory
	{
		/// <summary>
		/// Opens a new storage session.
		/// </summary>
		/// <returns>An open storage session</returns>
		IStorage Open();
	}
}
