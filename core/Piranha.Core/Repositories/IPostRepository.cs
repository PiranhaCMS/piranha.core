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
	/// The client post repository interface.
	/// </summary>
	public interface IPostRepository
	{
		/// <summary>
		/// Gets the post model with the specified id.
		/// </summary>
		/// <param name="id">The unique id</param>
		/// <returns>The post model</returns>
		PostModel GetById(Guid id);

		/// <summary>
		/// Gets the post model with the specified id.
		/// </summary>
		/// <param name="id">The unique id</param>
		/// <returns>The post model</returns>
		T GetById<T>(Guid id) where T : PostModel;

		/// <summary>
		/// Gets the post model with the specified slug.
		/// </summary
		/// <param name="categoryId">The category id</param>
		/// <param name="slug">The unique slug</param>
		/// <returns>The post model</returns>
		PostModel GetBySlug(Guid categoryId, string slug);

		/// <summary>
		/// Gets the post model with the specified slug.
		/// </summary
		/// <param name="categoryId">The category id</param>
		/// <param name="slug">The unique slug</param>
		/// <returns>The post model</returns>
		T GetBySlug<T>(Guid categoryId, string slug) where T : PostModel;

		/// <summary>
		/// Gets the latest post from the specified category.
		/// </summary>
		/// <param name="categoryId">The category id</param>
		/// <returns>The latest post</returns>
		PostModel GetLatest(Guid categoryId);

		/// <summary>
		/// Gets the latest post from the specified category.
		/// </summary>
		/// <param name="categoryId">The category id</param>
		/// <returns>The latest post</returns>
		T GetLatest<T>(Guid categoryId) where T : PostModel;
	}
}
