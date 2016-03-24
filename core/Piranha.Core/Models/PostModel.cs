/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Dynamic;

namespace Piranha.Models
{
	/// <summary>
	/// The client post model.
	/// </summary>
    public class PostModel : PostListModel
    {
		#region Properties
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
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public PostModel() {
			Regions = new ExpandoObject();
		}
	}
}
