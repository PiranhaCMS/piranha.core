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
using System.Linq;
using Piranha.Data;

namespace Piranha.Repositories
{
	/// <summary>
	/// The client category repository.
	/// </summary>
    public class CategoryRepository
    {
		#region Members
		/// <summary>
		/// The current db context.
		/// </summary>
		private readonly Db db;
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="db">The current db context</param>
		internal CategoryRepository(Db db) {
			this.db = db;
		}

		/// <summary>
		/// Gets the category identified by the given id.
		/// </summary>
		/// <param name="id">The unique id</param>
		/// <returns>The category</returns>
		public Category GetById(Guid id) {
			return db.Categories
				.SingleOrDefault(c => c.Id == id);
		}

		/// <summary>
		/// Gets the category identified by the given slug.
		/// </summary>
		/// <param name="id">The unique slug</param>
		/// <returns>The category</returns>
		public Category GetBySlug(string slug) {
			return db.Categories
				.SingleOrDefault(c => c.Slug == slug);
		}
	}
}
