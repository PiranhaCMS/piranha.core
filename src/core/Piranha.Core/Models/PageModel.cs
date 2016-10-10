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
    public class PageModel : PageBase
    {
		#region Properties
		/// <summary>
		/// Gets/sets the regions.
		/// </summary>
		public dynamic Regions { get; set; }

		/// <summary>
		/// Gets if this is the startpage of the site.
		/// </summary>
		public bool IsStartPage {
			get { return !ParentId.HasValue && SortOrder == 0; }
		}
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public PageModel() {
			Regions = new ExpandoObject();
		}
	}
}
