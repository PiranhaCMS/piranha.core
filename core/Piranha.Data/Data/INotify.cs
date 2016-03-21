/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha.Data
{
	/// <summary>
	/// Interface for models that should be notified before
	/// changes are saved.
	/// </summary>
    public interface INotify
    {
		/// <summary>
		/// Called right before the model is about to be saved.
		/// </summary>
		/// <param name="db">The current db context</param>
		void OnSave(Db db);

		/// <summary>
		/// Executed right before the model is about to be deleted.
		/// </summary>
		/// <param name="db">The current db context</param>
		void OnDelete(Db db);
    }
}
