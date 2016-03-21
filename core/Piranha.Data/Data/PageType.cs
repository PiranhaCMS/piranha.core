/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.dnx
 * 
 */

namespace Piranha.Data
{
	/// <summary>
	/// Page types are used to define reusable page templates.
	/// </summary>
	public sealed class PageType : Base.ContentType<PageType, PageTypeField>, INotify
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public PageType() : base() { }

		#region Notifications
		/// <summary>
		/// Called right before the model is about to be saved.
		/// </summary>
		/// <param name="db">The current db context</param>
		public void OnSave(Db db) {
			if (Hooks.Data.PageType.OnSave != null)
				Hooks.Data.PageType.OnSave(db, this);
		}

		/// <summary>
		/// Executed right before the model is about to be deleted.
		/// </summary>
		/// <param name="db">The current db context</param>
		public void OnDelete(Db db) {
			if (Hooks.Data.PageType.OnDelete != null)
				Hooks.Data.PageType.OnDelete(db, this);
		}
		#endregion
	}
}
