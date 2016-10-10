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
	/// The client category repository interface.
	/// </summary>
	public interface ICategoryRepository
	{
		/// <summary>
		/// Gets the category with the specified id.
		/// </summary>
		/// <param name="id">The unique id</param>
		/// <returns>The category</returns>
		Category GetById(Guid id);

		/// <summary>
		/// Gets the category with the specified slug.
		/// </summary>
		/// <param name="slug">The unique slug</param>
		/// <returns>The category</returns>
		Category GetBySlug(string slug);

		/// <summary>
		/// Gets the full category model with the specified id.
		/// </summary>
		/// <param name="id">The unique id</param>
		/// <returns>The category</returns>
		CategoryModel GetModelById(Guid id);

		/// <summary>
		/// Gets the full category model with the specified slug.
		/// </summary>
		/// <param name="slug">The unique slug</param>
		/// <returns>The category</returns>
		CategoryModel GetModelBySlug(string slug);

		void Save(Category category);
		void Save(CategoryModel category);
	}
}
