/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Piranha.Repositories;

namespace Piranha.EF.Repositories
{
	/// <summary>
	/// Page type repository.
	/// </summary>
    public class PageTypeRepository : IPageTypeRepository
    {
		#region Members
		private readonly Db db;
		#endregion

		/// <summary>
		/// Default internal constructor.
		/// </summary>
		/// <param name="db">The current db context</param>
		internal PageTypeRepository(Db db) {
			this.db = db;
		}

		/// <summary>
		/// Gets the page type with the specified id.
		/// </summary>
		/// <param name="id">The unique id</param>
		/// <returns>The page type</returns>
		public Models.PageType GetById(string id) {
			var type = db.PageTypes
				.SingleOrDefault(t => t.Id == id);
			
			if (type != null)
				return JsonConvert.DeserializeObject<Models.PageType>(type.Body);
			return null;
		}

		/// <summary>
		/// Gets all available page types.
		/// </summary>
		/// <returns>The page types</returns>
		public IList<Models.PageType> Get() {
			var result = new List<Models.PageType>();

			var types = db.PageTypes.ToList();
			foreach (var type in types)
				result.Add(JsonConvert.DeserializeObject<Models.PageType>(type.Body));

			return result.OrderBy(t => t.Title).ToList();
		}

		/// <summary>
		/// Saves the given page type.
		/// </summary>
		/// <param name="pageType">The page type</param>
		public void Save(Models.PageType pageType) {
			var type = db.PageTypes
				.SingleOrDefault(t => t.Id == pageType.Id);

			if (type == null) {
				type = new Data.PageType() {
					Id = pageType.Id
				};
				db.PageTypes.Add(type);
			}
			type.Body = JsonConvert.SerializeObject(pageType);

			db.SaveChanges();
		}

		/// <summary>
		/// Deletes the given page type.
		/// </summary>
		/// <param name="pageType">The page type</param>
		public void Delete(Models.PageType pageType) {
			Delete(pageType.Id);
		}

		/// <summary>
		/// Deletes the page type with the specified id.
		/// </summary>
		/// <param name="id">The unique id</param>
		public void Delete(string id) {
			var type = db.PageTypes
				.SingleOrDefault(t => t.Id == id);

			if (type != null) {
				db.PageTypes.Remove(type);
				db.SaveChanges();
			}
		}
	}
}
