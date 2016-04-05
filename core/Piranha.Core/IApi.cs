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
using System.Threading.Tasks;

namespace Piranha
{
	/// <summary>
	/// Interface for the main application Api.
	/// </summary>
    public interface IApi : IDisposable
    {
		/// <summary>
		/// Gets the archive repository.
		/// </summary>
		Repositories.IArchiveRepository Archives { get; }

		/// <summary>
		/// Gets the category repository.
		/// </summary>
		Repositories.ICategoryRepository Categories { get; }

		/// <summary>
		/// Gets the page repository.
		/// </summary>
		Repositories.IPageRepository Pages { get; }

		/// <summary>
		/// Gets the post repository.
		/// </summary>
		Repositories.IPostRepository Posts { get; }

		/// <summary>
		/// Gets the site map repository.
		/// </summary>
		Repositories.ISiteMapRepository SiteMap { get; }

		/// <summary>
		/// Saves the changes made to the api.
		/// </summary>
		/// <returns>The number of saved rows.</returns>
		int SaveChanges();

		/// <summary>
		/// Saves the changes made to the api.
		/// </summary>
		/// <returns>The number of saved rows.</returns>
		Task<int> SaveChangesAsync();
	}
}
