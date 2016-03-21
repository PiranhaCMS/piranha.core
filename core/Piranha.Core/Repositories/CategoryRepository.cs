/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using Piranha.Data;
using Piranha.Models;

namespace Piranha.Repositories
{
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

		public Category GetById(Guid id) {
			return db.Categories
				.SingleOrDefault(c => c.Id == id);
		}

		public Category GetBySlug(string slug) {
			return db.Categories
				.SingleOrDefault(c => c.Slug == slug);
		}
	}
}
