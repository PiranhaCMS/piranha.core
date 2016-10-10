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
    public class Post : Models.PostBase, IModel, ISlug, ICreated, IModified
    {
		#region Properties
		/// <summary>
		/// Gets/sets the category id.
		/// </summary>
		public Guid CategoryId { get; set; }
		#endregion

		#region Navigation properties
		/// <summary>
		/// Gets/sets the post category.
		/// </summary>
		public Category Category { get; set; }
		#endregion
	}
}
