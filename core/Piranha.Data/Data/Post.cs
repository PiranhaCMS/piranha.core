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

namespace Piranha.Data
{
	/// <summary>
	/// Posts are used to create content without a fixed position
	/// in the sitemap.
	/// </summary>
	public sealed class Post : Base.Content<Post, PostField, PostType, PostTypeField>, IMeta, INotify
	{
		#region Properties
		/// <summary>
		/// Gets/sets the category id.
		/// </summary>
		public Guid CategoryId { get; set; }

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
		/// Gets/sets the optional excerpt.
		/// </summary>
		public string Excerpt { get; set; }
		#endregion

		#region Navigation properties
		/// <summary>
		/// Gets/sets the category.
		/// </summary>
		public Category Category { get; set; }

		/// <summary>
		/// Gets/sets the available tags.
		/// 
		/// TODO: 
		/// Wait to implement this until EF7 supports implicit
		/// many-to-many relationships
		/// </summary>
		//public IList<Tag> Tags { get; set; } 
		#endregion

		#region Notifications
		/// <summary>
		/// Called right before the model is about to be saved.
		/// </summary>
		/// <param name="db">The current db context</param>
		public void OnSave(Db db) {
			if (Hooks.Data.Post.OnSave != null)
				Hooks.Data.Post.OnSave(db, this);
		}

		/// <summary>
		/// Executed right before the model is about to be deleted.
		/// </summary>
		/// <param name="db">The current db context</param>
		public void OnDelete(Db db) {
			if (Hooks.Data.Post.OnDelete != null)
				Hooks.Data.Post.OnDelete(db, this);
		}
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Post() : base() {
			//TODO: Tags = new List<Tag>();
		}
	}
}
