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
        /// Gets the post archive for the blog with the given id.
        /// </summary>
        /// <param name="id">The unique blog id</param>
        /// <param name="page">The optional page</param>
        /// <param name="year">The optional year</param>
        /// <param name="month">The optional month</param>
        /// <returns>The archive model</returns>
        public T GetById<T>(Guid id, int? page = 1, int? year = null, int? month = null) where T : Models.BlogPage<T> {
            // Get the requested blog page
            var model = api.Pages.GetById<T>(id);

            if (model != null) {
                // Set basic fields
                model.Archive = new Models.PostArchive();

                model.Route = model.Route ?? "/archive";
                model.Archive.Year = year;
                model.Archive.Month = month;
                model.Archive.CurrentPage = Math.Max(1, page.HasValue ? page.Value : 1);

                // Build the query.
                var now = DateTime.Now;
                var query = db.Posts
                    .Where(p => p.BlogId == id && p.Published <= now);

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
                model.Archive.TotalPages = Math.Max(Convert.ToInt32(Math.Ceiling((double)query.Count() / ArchivePageSize)), 1);
                model.Archive.CurrentPage = Math.Min(model.Archive.CurrentPage, model.Archive.TotalPages);

                // Get the posts
                var posts = query
                    .OrderByDescending(p => p.Published)
                    .Skip((model.Archive.CurrentPage - 1) * ArchivePageSize)
                    .Take(ArchivePageSize)
                    .Select(p => p.Id);

                // Map & add the posts within the requested page
                foreach (var post in posts) {
                    model.Archive.Posts.Add(api.Posts.GetById(post));
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
        /// <param name="siteId">The optional site id</param>
        /// <returns>The archive model</returns>
        public T GetBySlug<T>(string slug, int? page = 1, int? year = null, int? month = null, Guid? siteId = null) where T : Models.BlogPage<T> {
            // Get the id of the blog page with the given type
            var blogId = api.Pages.GetIdBySlug(slug, siteId);

            if (blogId.HasValue)
                return GetById<T>(blogId.Value, page, year, month);
            return null;
        }
    }
}
