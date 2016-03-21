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
	/// Interface for data models with a unique Guid id.
	/// </summary>
    public interface IModel
    {
		/// <summary>
		/// Gets/sets the unique id.
		/// </summary>
		Guid Id { get; set; }
    }
}
