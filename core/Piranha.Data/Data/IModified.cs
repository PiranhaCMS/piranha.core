/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.dnx
 * 
 */

using System;

namespace Piranha.Data
{
	/// <summary>
	/// Interface for data models tracking last modification date.
	/// </summary>
    public interface IModified
    {
		/// <summary>
		/// Gets/sets the last modification date.
		/// </summary>
		DateTime LastModified { get; set; }
    }
}
