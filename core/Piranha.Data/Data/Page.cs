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

namespace Piranha.Data
{
	/// <summary>
	/// Pages are used to create content with a fixed position
	/// in the sitemap.
	/// </summary>
	public sealed class Page : Base.Content<Page, PageField, PageType, PageTypeField>, IMeta, INotify
    {
		#region Properties
		/// <summary>
		/// Gets/sets the optional navigation title.
		/// </summary>
		public string NavigationTitle { get; set; }

		/// <summary>
		/// Gets/sets the optional meta title.
		/// </summary>
		public string MetaTitle { get; set; }

		/// <summary>
		/// Gets/sets the optional meta keywords.
		/// </summary>
		public string MetaKeywords { get; set; }

		/// <summary>
		/// Gets/sets the optional meta description.
		/// </summary>
		public string MetaDescription { get; set; }

		/// <summary>
		/// Gets/sets the optional parent id.
		/// </summary>
		public Guid? ParentId { get; set; }

		/// <summary>
		/// Gets/sets the sort order.
		/// </summary>
		public int SortOrder { get; set; }

		/// <summary>
		/// Gets/sets if the page should be hidden in the navigation.
		/// </summary>
		public bool IsHidden { get; set; }
		#endregion

		#region Notifications
		/// <summary>
		/// Called right before the model is about to be saved.
		/// </summary>
		/// <param name="db">The current db context</param>
		public void OnSave(Db db) {
			if (Hooks.Data.Page.OnSave != null)
				Hooks.Data.Page.OnSave(db, this);
		}

		/// <summary>
		/// Executed right before the model is about to be deleted.
		/// </summary>
		/// <param name="db">The current db context</param>
		public void OnDelete(Db db) {
			if (Hooks.Data.Page.OnDelete != null)
				Hooks.Data.Page.OnDelete(db, this);
		}
		#endregion
	}
}
