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
using Piranha.Data;

namespace Piranha.Repositories
{
	/// <summary>
	/// The client category repository interface.
	/// </summary>
	public interface ICategoryRepository
	{
		/// <summary>
		/// Gets the category identified by the given id.
		/// </summary>
		/// <param name="id">The unique id</param>
		/// <returns>The category</returns>
		Category GetById(Guid id);

		/// <summary>
		/// Gets the category identified by the given slug.
		/// </summary>
		/// <param name="id">The unique slug</param>
		/// <returns>The category</returns>
		Category GetBySlug(string slug);
	}
}
