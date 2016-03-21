/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.dnx
 * 
 */

namespace Piranha.Data
{
	/// <summary>
	/// Interface for data models with a unique internal id.
	/// </summary>
    public interface IInternalId
    {
		/// <summary>
		/// Gets/sets the display name.
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Gets/sets the unique internal id.
		/// </summary>
		string InternalId { get; set; }
    }
}
