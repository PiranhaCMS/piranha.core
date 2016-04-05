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
using System.Linq;
using Piranha.Models;

namespace Piranha.Repositories
{
	/// <summary>
	/// The client archive repository.
	/// </summary>
    public class ArchiveRepository : IArchiveRepository
    {
		#region Members
		/// <summary>
		/// The current db context.
		/// </summary>
		private readonly Db db;

		/// <summary>
		/// TODO: This should be configurable.
		/// </summary>
		private const int ArchivePageSize = 5;
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="db">The current db context</param>
		internal ArchiveRepository(Db db) {
			this.db = db;
		}

		/// <summary>
		/// Gets the archive model for the specified category & period.
		/// </summary>
		/// <param name="id">The unique category id</param>
		/// <param name="page">The optional archive page</param>
		/// <param name="year">The optional year</param>
		/// <param name="month">The optional month</param>
		/// <returns>The model</returns>
		public ArchiveModel GetByCategoryId(Guid id, int? page = null, int? year = null, int? month = null) {
			// Get the requested category
			var category = db.Categories
				.SingleOrDefault(c => c.Id == id);

			if (category != null) {
				// Set basic fields
				var model = new ArchiveModel() {
					Id = id,
					Title = category.Title,
					Slug = category.Slug,
					Description = category.Description,
					Route = !String.IsNullOrEmpty(category.ArchiveRoute) ?
						category.ArchiveRoute : "/archive",
					Year = year,
					Month = month,
					Page = page.HasValue ? page.Value : 1
				};

				// Built the query.
				var now = DateTime.Now;
				var query = db.Posts
					.Include(p => p.Author)
					.Include(p => p.Category)
					.Where(p => p.CategoryId == id && p.Published <= now);

				if (year.HasValue) {
					DateTime from;
					DateTime to;

					if (month.HasValue) {
						from = new DateTime(year.Value, month.Value, 1);
						to = from.AddMonths(1);
					} else {
						from = new DateTime(year.Value, 1, 1);
						to = from.AddYears(1);
					}
					query = query.Where(p => p.Published >= from && p.Published < to);
				}

				// Get the total page count for the archive
				model.PageCount = Math.Max(Convert.ToInt32(Math.Ceiling((double)query.Count() / ArchivePageSize)), 1);
				model.Page = Math.Min(model.Page, model.PageCount);

				// Get the posts
				var posts = query
					.OrderBy(p => p.Published)
					.Take(model.PageCount * ArchivePageSize)
					.ToList();

				// Map & add the posts within the requested page
				var mapper = App.Mapper;
				for (var n = (model.Page - 1) * ArchivePageSize; n < Math.Min(model.Page * ArchivePageSize, posts.Count); n++) {
					// Map fields
					var post = mapper.Map<Data.Post, PostListModel>(posts[n]);
					post.Permalink = $"~/{category.Slug}/{post.Slug}";

					// Add to model
					model.Posts.Add(post);
				}
				return model;
			}
			return null;
		}
	}
}
