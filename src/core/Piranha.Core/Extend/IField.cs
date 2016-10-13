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
	/// Interface for fields.
	/// </summary>
    public interface IField
    {
		/// <summary>
		/// Initializes the field for client use.
		/// </summary>
		void Init();

		/// <summary>
		/// Initializes the field for manager use. This
		/// method can be used for loading additional meta
		/// data needed.
		/// </summary>
		void InitManager();

		/// <summary>
		/// Gets the client value.
		/// </summary>
		/// <returns></returns>
		object GetValue();
    }
}
