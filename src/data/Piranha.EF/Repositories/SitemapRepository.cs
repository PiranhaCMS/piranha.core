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
using System.Collections.Generic;
using System.Linq;
using Piranha.Repositories;

namespace Piranha.EF.Repositories
{
    public class SitemapRepository : ISitemapRepository
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
        internal SitemapRepository(Db db) {
            this.db = db;
        }
        /// <summary>
        /// Gets the hierachical sitemap structure.
        /// </summary>
        /// <param name="onlyPublished">If only published items should be included</param>
        /// <returns>The sitemap</returns>
        public IList<Models.SitemapItem> Get(bool onlyPublished = true) {
            IQueryable<Data.Page> query = db.Pages;
            if (onlyPublished)
                query = query.Where(p => p.Published <= DateTime.Now);

            var pages = query
                .OrderBy(p => p.ParentId)
                .ThenBy(p => p.SortOrder)
                .ToList();

            return Sort(pages);
        }

        #region Private methods
        /// <summary>
        /// Sorts the items.
        /// </summary>
        /// <param name="pages">The full page list</param>
        /// <param name="parentId">The current parent id</param>
        /// <returns>The sitemap</returns>
        private IList<Models.SitemapItem> Sort(IList<Data.Page> pages, Guid? parentId = null, int level = 0) {
            var result = new List<Models.SitemapItem>();

            foreach (var page in pages.Where(p => p.ParentId == parentId).OrderBy(p => p.SortOrder)) {
                var item = new Models.SitemapItem() {
                    Id = page.Id,
                    Title = page.Title,
                    NavigationTitle = page.NavigationTitle,
                    Permalink = page.Slug,
                    Level = level,
                    Published = page.Published,
                    Created = page.Created,
                    LastModified = page.LastModified
                };
                item.Items = Sort(pages, page.Id, level + 1);

                result.Add(item);
            }
            return result;
        }
        #endregion
    }
}
