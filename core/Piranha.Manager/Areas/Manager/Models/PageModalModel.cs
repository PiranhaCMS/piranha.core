/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Piranha.Manager;
using Piranha.Models;

namespace Piranha.Areas.Manager.Models
{
    /// <summary>
    /// The page edit view model.
    /// </summary>
    public class PageModalModel
    {
        public class SiteItem
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
        }

        public Sitemap Sitemap { get; set; }
        public IEnumerable<SiteItem> Sites { get; set; }
        public Guid SiteId { get; set; }
        public string SiteTitle { get; set; }
        public int ExpandedLevels { get; set; }

        public PageModalModel() {
            Sites = new List<SiteItem>();
        }

        public static PageModalModel GetBySiteId(IApi api, Guid? siteId = null) {
            var model = new PageModalModel();

            // Get default site if none is selected
            if (!siteId.HasValue) {
                var site = api.Sites.GetDefault();

                if (site != null)
                    siteId = site.Id;
            }
            model.SiteId = siteId.Value;

            // Get the sites available
            model.Sites = api.Sites.GetAll()
                .Select(s => new SiteItem() {
                    Id = s.Id,
                    Title = s.Title
                }).OrderBy(s => s.Title).ToList();

            // Get the current site title
            var currentSite = model.Sites.FirstOrDefault(s => s.Id == siteId.Value);
            if (currentSite != null)
                model.SiteTitle = currentSite.Title;

            // Get the sitemap
            model.Sitemap = api.Sites.GetSitemap(siteId, true);

            // Gets the expanded levels from config
            using (var config = new Config(api)) {
                model.ExpandedLevels = config.ManagerExpandedSitemapLevels;
            }

            return model;
        }
    }
}