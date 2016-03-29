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
using System.Collections.Generic;
using System.Linq;
using Piranha.Models;

namespace Piranha.Repositories
{
	/// <summary>
	/// The client site map repository.
	/// </summary>
	public class SiteMapRepository
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
		internal SiteMapRepository(Db db) {
			this.db = db;
		}

		/// <summary>
		/// Gets the current sitemap.
		/// </summary>
		/// <returns>The hierarchical sitemap</returns>
		public IList<SiteMapModel> Get() {
			var pages = db.Pages
				.Where(p => p.Published <= DateTime.Now)
				.OrderBy(p => p.ParentId)
				.ThenBy(p => p.SortOrder)
				.ToList();

			return Sort(pages);
		}


		/// <summary>
		/// Sorts the page structure recursive.
		/// </summary>
		/// <param name="pages">The pages</param>
		/// <param name="parentId">The parent id</param>
		/// <returns>The sitemap structure</returns>
		private List<SiteMapModel> Sort(List<Data.Page> pages, Guid? parentId = null, int level = 1) {
			List<SiteMapModel> ret = new List<SiteMapModel>();

			foreach (var page in pages) {
				if (page.ParentId == parentId) {
					var sm = App.Mapper.Map<Data.Page, SiteMapModel>(page);

					sm.Permalink = $"~/{page.Slug}";
					sm.Level = level;
					sm.Children = Sort(pages, page.Id, level + 1);
					ret.Add(sm);
				}
			}
			return ret;
		}
	}
}
