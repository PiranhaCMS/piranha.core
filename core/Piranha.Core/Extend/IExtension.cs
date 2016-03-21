/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha.Extend
{
	/// <summary>
	/// Interface for all extensions.
	/// </summary>
    public interface IExtension
    {
		/// <summary>
		/// Gets the value for the client model.
		/// </summary>
		/// <returns>The value</returns>
		object GetValue();
    }
}
