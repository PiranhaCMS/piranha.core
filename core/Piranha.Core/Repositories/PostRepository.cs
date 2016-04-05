/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using Piranha.Data;
using Piranha.Models;

namespace Piranha.Repositories
{
	/// <summary>
	/// The client post repository.
	/// </summary>
    public class PostRepository : IPostRepository
    {
		#region Members
		/// <summary>
		/// The current db context.
		/// </summary>
		private readonly Db db;
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="db">The current db context</param>
		internal PostRepository(Db db) {
			this.db = db;
		}

		/// <summary>
		/// Gets the post model with the specified id.
		/// </summary>
		/// <param name="id">The unique id</param>
		/// <returns>The post model</returns>
		public PostModel GetById(Guid id) {
			return GetById<PostModel>(id);
		}

		/// <summary>
		/// Gets the post model with the specified id.
		/// </summary>
		/// <param name="id">The unique id</param>
		/// <returns>The post model</returns>
		public T GetById<T>(Guid id) where T : PostModel {
			var post = FullQuery()
				.Where(p => p.Id == id && p.Published <= DateTime.Now)
				.SingleOrDefault();

			if (post != null)
				return FullTransform<T>(post);
			return null;
		}

		/// <summary>
		/// Gets the post model with the specified slug.
		/// </summary
		/// <param name="categoryId">The category id</param>
		/// <param name="slug">The unique slug</param>
		/// <returns>The post model</returns>
		public PostModel GetBySlug(Guid categoryId, string slug) {
			return GetBySlug<PostModel>(categoryId, slug);
		}

		/// <summary>
		/// Gets the post model with the specified slug.
		/// </summary
		/// <param name="categoryId">The category id</param>
		/// <param name="slug">The unique slug</param>
		/// <returns>The post model</returns>
		public T GetBySlug<T>(Guid categoryId, string slug) where T : PostModel {
			var post = FullQuery()
				.Where(p => p.CategoryId == categoryId && p.Slug == slug && p.Published <= DateTime.Now)
				.SingleOrDefault();

			if (post != null)
				return FullTransform<T>(post);
			return null;
		}

		/// <summary>
		/// Gets the latest post from the specified category.
		/// </summary>
		/// <param name="categoryId">The category id</param>
		/// <returns>The latest post</returns>
		public PostModel GetLatest(Guid categoryId) {
			return GetLatest<PostModel>(categoryId);
		}

		/// <summary>
		/// Gets the latest post from the specified category.
		/// </summary>
		/// <param name="categoryId">The category id</param>
		/// <returns>The latest post</returns>
		public T GetLatest<T>(Guid categoryId) where T : PostModel {
			var post = FullQuery()
				.Where(p => p.CategoryId == categoryId && p.Published <= DateTime.Now)
				.OrderByDescending(p => p.Published)
				.FirstOrDefault();

			if (post != null)
				return FullTransform<T>(post);
			return null;
		}

		#region Private methods
		/// <summary>
		/// Gets the full post query.
		/// </summary>
		/// <returns>The queryable</returns>
		private IQueryable<Post> FullQuery() {
			return db.Posts
				.Include(p => p.Author)
				.Include(p => p.Category)
				.Include(p => p.Type.Fields)
				.Include(p => p.Fields);
		}

		/// <summary>
		/// Transforms the given post into a full model.
		/// </summary>
		/// <param name="post">The post</param>
		/// <returns>The transformed model</returns>
		private T FullTransform<T>(Post post) where T : PostModel {
			// Create the post model
			var model = Activator.CreateInstance<T>();

			// Map basic fields
			App.Mapper.Map<Post, PostModel>(post, model);

			// Map additional fields
			model.Route = !String.IsNullOrEmpty(post.Route) ? post.Route :
				!String.IsNullOrEmpty(post.Type.Route) ? post.Type.Route : "/post";
			model.Permalink = $"~/{post.Category.Slug}/{post.Slug}";

			// Map regions
			foreach (var fieldType in post.Type.Fields.Where(f => f.FieldType == FieldType.Region)) {
				var field = post.Fields.SingleOrDefault(f => f.TypeId == fieldType.Id);

				if (field != null) {
					var region = App.ExtensionManager.Deserialize(fieldType.CLRType, field.Value);
					if (region != null)
						((IDictionary<string, object>)model.Regions)[fieldType.InternalId] = region.GetValue();
				}
			}
			return model;
		}
		#endregion
	}
}
