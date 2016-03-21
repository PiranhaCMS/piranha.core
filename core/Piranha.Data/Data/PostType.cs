/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha.Data
{
	/// <summary>
	/// Post types are used to define reusable post templates.
	/// </summary>
	public sealed class PostType : Base.ContentType<PostType, PostTypeField>, INotify
    {
		/// <summary>
		/// Default constructor.
		/// </summary>
		public PostType() : base() { }

		#region Notifications
		/// <summary>
		/// Called right before the model is about to be saved.
		/// </summary>
		/// <param name="db">The current db context</param>
		public void OnSave(Db db) {
			if (Hooks.Data.PostType.OnSave != null)
				Hooks.Data.PostType.OnSave(db, this);
		}

		/// <summary>
		/// Executed right before the model is about to be deleted.
		/// </summary>
		/// <param name="db">The current db context</param>
		public void OnDelete(Db db) {
			if (Hooks.Data.PostType.OnDelete != null)
				Hooks.Data.PostType.OnDelete(db, this);
		}
		#endregion
	}
}
