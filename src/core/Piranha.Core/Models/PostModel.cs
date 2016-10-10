/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Collections.Generic;

namespace Piranha.Models
{
    public class PostModel : PostBase
    {
		#region Properties
		/// <summary>
		/// Gets/sets the post category.
		/// </summary>
		public Category Category { get; set; }

		/// <summary>
		/// Gets/sets the available tags.
		/// </summary>
		public IList<Tag> Tags { get; set; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public PostModel() {
			Tags = new List<Tag>();
		}
	}
}
