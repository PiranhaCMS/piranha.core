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
	/// An application config parameter.
	/// </summary>
    public sealed class Param : IModel, IInternalId, ICreated, IModified, INotify
    {
		#region Properties
		/// <summary>
		/// Gets/sets the unique id.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets/sets the display name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets/sets the unique internal id.
		/// </summary>
		public string InternalId { get; set; }

		/// <summary>
		/// Gets/sets the value.
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// Gets/sets the created date.
		/// </summary>
		public DateTime Created { get; set; }

		/// <summary>
		/// Gets/sets the last modified date.
		/// </summary>
		public DateTime LastModified { get; set; }
		#endregion

		#region Notifications
		/// <summary>
		/// Called right before the model is about to be saved.
		/// </summary>
		/// <param name="db">The current db context</param>
		public void OnSave(Db db) {
			if (Hooks.Data.Param.OnSave != null)
				Hooks.Data.Param.OnSave(db, this);
		}

		/// <summary>
		/// Executed right before the model is about to be deleted.
		/// </summary>
		/// <param name="db">The current db context</param>
		public void OnDelete(Db db) {
			if (Hooks.Data.Param.OnDelete != null)
				Hooks.Data.Param.OnDelete(db, this);
		}
		#endregion
	}
}
