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
using System.Dynamic;

namespace Piranha.Models
{
	/// <summary>
	/// The client page model.
	/// </summary>
    public class PageModel
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
		/// Gets/sets the navigation title.
		/// </summary>
		public string NavigationTitle { get; set; }

		/// <summary>
		/// Gets/sets the unique slug.
		/// </summary>
		public string Slug { get; set; }

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
		/// Gets/sets the available regions.
		/// </summary>
		public dynamic Regions { get; set; }

		/// <summary>
		/// Gets/sets the internal route used by the middleware.
		/// </summary>
		internal string Route { get; set; }

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

		/// <summary>
		/// Default constructor.
		/// </summary>
		public PageModel() {
			Regions = new ExpandoObject();
		}
	}
}
