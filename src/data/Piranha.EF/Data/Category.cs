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

namespace Piranha.EF.Data
{
    public class Category : Models.Category, IModel, ISlug, ICreated, IModified
    {
		#region Properties
		/// <summary>
		/// Gets/sets the archive title.
		/// </summary>
		public string ArchiveTitle { get; set; }

		/// <summary>
		/// Gets/sets the archive meta keywords.
		/// </summary>
		public string ArchiveKeywords { get; set; }

		/// <summary>
		/// Gets/sets the archive meta description.
		/// </summary>
		public string ArchiveDescription { get; set; }

		/// <summary>
		/// Gets/sets the archive route.
		/// </summary>
		public string ArchiveRoute { get; set; }

		/// <summary>
		/// Gets/sets the created date.
		/// </summary>
		public DateTime Created { get; set; }

		/// <summary>
		/// Gets/sets the last modification date.
		/// </summary>
		public DateTime LastModified { get; set; }
		#endregion
	}
}
