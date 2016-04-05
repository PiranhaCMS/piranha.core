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
using Piranha.Models;

namespace Piranha.Repositories
{
	/// <summary>
	/// The client archive repository interface.
	/// </summary>
	public interface IArchiveRepository
	{
		/// <summary>
		/// Gets the archive model for the specified category & period.
		/// </summary>
		/// <param name="id">The unique category id</param>
		/// <param name="page">The optional archive page</param>
		/// <param name="year">The optional year</param>
		/// <param name="month">The optional month</param>
		/// <returns>The model</returns>
		ArchiveModel GetByCategoryId(Guid id, int? page = null, int? year = null, int? month = null);
	}
}
