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

namespace Piranha.Models
{
	/// <summary>
	/// Client post list model.
	/// </summary>
    public class PostListModel
    {
		#region Properties
		/// <summary>
		/// Gets/sets the unique id.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets/sets the main title.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Gets/sets the unique slug.
		/// </summary>
		public string Slug { get; set; }

		/// <summary>
		/// Gets/sets the public permalink.
		/// </summary>
		public string Permalink { get; set; }

		/// <summary>
		/// Gets/sets the optional excerpt.
		/// </summary>
		public string Excerpt { get; set; }

		/// <summary>
		/// Gets/sets the optional published date.
		/// </summary>
		public DateTime Published { get; set; }

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
