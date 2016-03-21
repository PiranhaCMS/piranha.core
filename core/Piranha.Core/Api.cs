/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.dnx
 * 
 */

using System;
using System.Threading.Tasks;

namespace Piranha
{
	/// <summary>
	/// The main application api.
	/// </summary>
    public sealed class Api : IDisposable
    {
		#region Members
		/// <summary>
		/// The current db context.
		/// </summary>
		private readonly Db db;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the category repository.
		/// </summary>
		public Repositories.CategoryRepository Categories { get; private set; }

		/// <summary>
		/// Gets the page repository.
		/// </summary>
		public Repositories.PageRepository Pages { get; private set; }

		/// <summary>
		/// Gets the post repository.
		/// </summary>
		public Repositories.PostRepository Posts { get; private set; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Api() {
			db = new Db();

			Categories = new Repositories.CategoryRepository(db);
			Pages = new Repositories.PageRepository(db);
			Posts = new Repositories.PostRepository(db);
		}

		/// <summary>
		/// Saves the changes made to the api.
		/// </summary>
		/// <returns>The number of saved rows.</returns>
		public int SaveChanges() {
			return db.SaveChanges();
		}

		/// <summary>
		/// Saves the changes made to the api.
		/// </summary>
		/// <returns>The number of saved rows.</returns>
		public Task<int> SaveChangesAsync() {
			return db.SaveChangesAsync();
		}

		/// <summary>
		/// Disposes the api.
		/// </summary>
		public void Dispose() {
			db.Dispose();
			GC.SuppressFinalize(this);
		}
    }
}
