/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.dnx
 * 
 */

namespace Piranha.Hooks
{
	/// <summary>
	/// The hooks available for the data layer.
	/// </summary>
    public static class Data
    {
		/// <summary>
		/// The delegates used.
		/// </summary>
		public static class Delegates
		{
			/// <summary>
			/// Delegate for notifying a model change.
			/// </summary>
			/// <typeparam name="T">The model type</typeparam>
			/// <param name="db">The current db context</param>
			/// <param name="model">The model</param>
			public delegate void OnNotifyDelegate<T>(Db db, T model);
		}

		/// <summary>
		/// The hooks available for the author model.
		/// </summary>
		public static class Author
		{
			/// <summary>
			/// Called right before the author is saved.
			/// </summary>
			public static Delegates.OnNotifyDelegate<Piranha.Data.Author> OnSave;

			/// <summary>
			/// Called right before the author is deleted.
			/// </summary>
			public static Delegates.OnNotifyDelegate<Piranha.Data.Author> OnDelete;
		}

		/// <summary>
		/// The hooks available for the category model.
		/// </summary>
		public static class Category
		{
			/// <summary>
			/// Called right before the category is saved.
			/// </summary>
			public static Delegates.OnNotifyDelegate<Piranha.Data.Category> OnSave;

			/// <summary>
			/// Called right before the category is deleted.
			/// </summary>
			public static Delegates.OnNotifyDelegate<Piranha.Data.Category> OnDelete;
		}

		/// <summary>
		/// The hooks available for the media model.
		/// </summary>
		public static class Media
		{
			/// <summary>
			/// Called right before the media is saved.
			/// </summary>
			public static Delegates.OnNotifyDelegate<Piranha.Data.Media> OnSave;

			/// <summary>
			/// Called right before the media is deleted.
			/// </summary>
			public static Delegates.OnNotifyDelegate<Piranha.Data.Media> OnDelete;
		}

		/// <summary>
		/// The hooks available for the media folder model.
		/// </summary>
		public static class MediaFolder
		{
			/// <summary>
			/// Called right before the media folder is saved.
			/// </summary>
			public static Delegates.OnNotifyDelegate<Piranha.Data.MediaFolder> OnSave;

			/// <summary>
			/// Called right before the media folder is deleted.
			/// </summary>
			public static Delegates.OnNotifyDelegate<Piranha.Data.MediaFolder> OnDelete;
		}

		/// <summary>
		/// The hooks available for the page model.
		/// </summary>
		public static class Page
		{
			/// <summary>
			/// Called right before the page is saved.
			/// </summary>
			public static Delegates.OnNotifyDelegate<Piranha.Data.Page> OnSave;

			/// <summary>
			/// Called right before the page is deleted.
			/// </summary>
			public static Delegates.OnNotifyDelegate<Piranha.Data.Page> OnDelete;
		}

		/// <summary>
		/// The hooks available for the page type model.
		/// </summary>
		public static class PageType
		{
			/// <summary>
			/// Called right before the page type is saved.
			/// </summary>
			public static Delegates.OnNotifyDelegate<Piranha.Data.PageType> OnSave;

			/// <summary>
			/// Called right before the page type is deleted.
			/// </summary>
			public static Delegates.OnNotifyDelegate<Piranha.Data.PageType> OnDelete;
		}

		/// <summary>
		/// The hooks available for the param model.
		/// </summary>
		public static class Param
		{
			/// <summary>
			/// Called right before the param is saved.
			/// </summary>
			public static Delegates.OnNotifyDelegate<Piranha.Data.Param> OnSave;

			/// <summary>
			/// Called right before the param is deleted.
			/// </summary>
			public static Delegates.OnNotifyDelegate<Piranha.Data.Param> OnDelete;
		}

		/// <summary>
		/// The hooks available for the post model.
		/// </summary>
		public static class Post
		{
			/// <summary>
			/// Called right before the post is saved.
			/// </summary>
			public static Delegates.OnNotifyDelegate<Piranha.Data.Post> OnSave;

			/// <summary>
			/// Called right before the post is deleted.
			/// </summary>
			public static Delegates.OnNotifyDelegate<Piranha.Data.Post> OnDelete;
		}

		/// <summary>
		/// The hooks available for the post type model.
		/// </summary>
		public static class PostType
		{
			/// <summary>
			/// Called right before the post type is saved.
			/// </summary>
			public static Delegates.OnNotifyDelegate<Piranha.Data.PostType> OnSave;

			/// <summary>
			/// Called right before the post type is deleted.
			/// </summary>
			public static Delegates.OnNotifyDelegate<Piranha.Data.PostType> OnDelete;
		}

		/// <summary>
		/// The hooks available for the tag model.
		/// </summary>
		public static class Tag
		{
			/// <summary>
			/// Called right before the tag is saved.
			/// </summary>
			public static Delegates.OnNotifyDelegate<Piranha.Data.Tag> OnSave;

			/// <summary>
			/// Called right before the tag is deleted.
			/// </summary>
			public static Delegates.OnNotifyDelegate<Piranha.Data.Tag> OnDelete;
		}
	}
}
