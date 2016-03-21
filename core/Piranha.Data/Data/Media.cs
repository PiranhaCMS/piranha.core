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

namespace Piranha.Data
{
	/// <summary>
	/// An uploaded media file.
	/// </summary>
    public sealed class Media : IModel, ICreated, IModified, INotify
    {
		#region Properties
		/// <summary>
		/// Gets/sets the unique id.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets/sets the optional folder id.
		/// </summary>
		public Guid? FolderId { get; set; }

		/// <summary>
		/// Gets/sets the filename.
		/// </summary>
		public string Filename { get; set; }

		/// <summary>
		/// Gets/sets the content type.
		/// </summary>
		public string ContentType { get; set; }

		/// <summary>
		/// Gets/sets the size of the uploaded asset.
		/// </summary>
		public long Size { get; set; }

		/// <summary>
		/// Gets/sets the public url.
		/// </summary>
		public string PublicUrl { get; set; }

		/// <summary>
		/// Gets/sets the created date.
		/// </summary>
		public DateTime Created { get; set; }

		/// <summary>
		/// Gets/sets the last modified date.
		/// </summary>
		public DateTime LastModified { get; set; }
		#endregion

		#region Navigation properties
		/// <summary>
		/// Gets/sets the optional folder.
		/// </summary>
		public MediaFolder Folder { get; set; }
		#endregion

		#region Notifications
		/// <summary>
		/// Called right before the model is about to be saved.
		/// </summary>
		/// <param name="db">The current db context</param>
		public void OnSave(Db db) {
			if (Hooks.Data.Media.OnSave != null)
				Hooks.Data.Media.OnSave(db, this);
		}

		/// <summary>
		/// Executed right before the model is about to be deleted.
		/// </summary>
		/// <param name="db">The current db context</param>
		public void OnDelete(Db db) {
			if (Hooks.Data.Media.OnDelete != null)
				Hooks.Data.Media.OnDelete(db, this);
		}
		#endregion
	}
}
