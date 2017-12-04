/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.EntityFrameworkCore;
using Piranha.Data;
using System;
using System.Linq;

namespace Piranha.Repositories
{
    public class ArchiveRepository : IArchiveRepository
    {
        #region Members
        /// <summary>
        /// The current api.
        /// </summary>
        private readonly Api api;

        /// <summary>
        /// The current db context.
        /// </summary>
        private readonly IDb db;

        /// <summary>
        /// TODO: This should be configurable.
        /// </summary>
        private const int ArchivePageSize = 5;
        #endregion

        /// <summary>
        /// Default internal constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        internal ArchiveRepository(Api api, IDb db) {
            this.api = api;
            this.db = db;
        }

        /// <summary>
        /// Gets the post archive for the category with the given id.
        /// </summary>
        /// <param name="id">The unique category id</param>
        /// <param name="page">The optional page</param>
        /// <param name="year">The optional year</param>
        /// <param name="month">The optional month</param>
        /// <returns>The archive model</returns>
        public Models.PostArchive GetById(Guid id, int? page = 1, int? year = null, int? month = null) {
            return GetById<Models.PostArchive>(id, page, year, month);
        }

        /// <summary>
        /// Gets the post archive for the category with the given id.
        /// </summary>
        /// <param name="id">The unique category id</param>
        /// <param name="page">The optional page</param>
        /// <param name="year">The optional year</param>
        /// <param name="month">The optional month</param>
        /// <returns>The archive model</returns>
        public T GetById<T>(Guid id, int? page = 1, int? year = null, int? month = null) where T : Models.PostArchive {
            // Get the requested category
            var category = db.Categories
                .FirstOrDefault(c => c.Id == id);

            if (category != null) {
                // Set basic fields
                var model = Activator.CreateInstance<T>();

                model.CategoryId = id;
                model.Title = category.ArchiveTitle ?? category.Title;
                model.Slug = category.Slug;
                model.MetaDescription = category.ArchiveDescription ?? category.Description;
                model.Route = !String.IsNullOrEmpty(category.ArchiveRoute) ? category.ArchiveRoute : "/archive";
                model.Year = year;
                model.Month = month;
                model.Page = Math.Max(1, page.HasValue ? page.Value : 1);

                // Build the query.
                var now = DateTime.Now;
                var query = db.Posts
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
                model.TotalPages = Math.Max(Convert.ToInt32(Math.Ceiling((double)query.Count() / ArchivePageSize)), 1);
                model.Page = Math.Min(model.Page, model.TotalPages);

                // Get the posts
                var posts = query
                    .OrderBy(p => p.Published)
                    .Skip((model.Page - 1) * ArchivePageSize)
                    .Take(ArchivePageSize)
                    .Select(p => p.Id);

                // Map & add the posts within the requested page
                foreach (var post in posts) {
                    model.Posts.Add(api.Posts.GetById(post));
                }
                return model;
            }
            return null;
        }

        /// <summary>
        /// Gets the post archive for the category with the given slug.
        /// </summary>
        /// <param name="slug">The unique category slug</param>
        /// <param name="page">The optional page</param>
        /// <param name="year">The optional year</param>
        /// <param name="month">The optional month</param>
        /// <returns>The archive model</returns>
        public Models.PostArchive GetBySlug(string slug, int? page = 1, int? year = null, int? month = null) {
            var category = db.Categories
                .FirstOrDefault(c => c.Slug == slug);

            if (category != null) {
                return GetById<Models.PostArchive>(category.Id, page, year, month);
            }
            return null;
        }

        /// <summary>
        /// Gets the post archive for the category with the given slug.
        /// </summary>
        /// <param name="slug">The unique category slug</param>
        /// <param name="page">The optional page</param>
        /// <param name="year">The optional year</param>
        /// <param name="month">The optional month</param>
        /// <returns>The archive model</returns>
        public T GetBySlug<T>(string slug, int? page = 1, int? year = null, int? month = null) where T : Models.PostArchive {
            var category = db.Categories
                .FirstOrDefault(c => c.Slug == slug);

            if (category != null) {
                return GetById<T>(category.Id, page, year, month);
            }
            return null;
        }
    }
}
