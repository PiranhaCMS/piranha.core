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
	/// Categories are used to organize posts.
	/// </summary>
    public sealed class Category : Base.Taxonomy, IModified, INotify
    {
		#region Properties
		/// <summary>
		/// Gets/sets the optional description.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets/sets if the default archive should be
		/// enabled for this category.
		/// </summary>
		public bool HasArchive { get; set; }

		/// <summary>
		/// Gets/sets the optional archive title.
		/// </summary>
		public string ArchiveTitle { get; set; }

		/// <summary>
		/// Gets/sets the optional archive route.
		/// </summary>
		public string ArchiveRoute { get; set; }

		/// <summary>
		/// Gets/sets the last modification date.
		/// </summary>
		public DateTime LastModified { get; set; }
		#endregion

		#region Notifications
		/// <summary>
		/// Called right before the model is about to be saved.
		/// </summary>
		/// <param name="db">The current db context</param>
		public void OnSave(Db db) {
			if (Hooks.Data.Category.OnSave != null)
				Hooks.Data.Category.OnSave(db, this);
		}

		/// <summary>
		/// Executed right before the model is about to be deleted.
		/// </summary>
		/// <param name="db">The current db context</param>
		public void OnDelete(Db db) {
			if (Hooks.Data.Category.OnDelete != null)
				Hooks.Data.Category.OnDelete(db, this);
		}
		#endregion
	}
}
