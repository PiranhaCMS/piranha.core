/*
 * Copyright (c) 2019 Håkan Edling
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
using System.Threading.Tasks;
using Piranha.Models;
using Piranha.Manager.Models;

namespace Piranha.Manager.Services
{
    public class PageService
    {
        private readonly IApi _api;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public PageService(IApi api)
        {
            _api = api;
        }

        /// <summary>
        /// Gets the list model.
        /// </summary>
        /// <returns>The list model</returns>
        public async Task<PageListModel> GetList()
        {
            var model = new PageListModel
            {
                Sites = (await _api.Sites.GetAllAsync()).Select(s => new PageListModel.SiteItem
                {
                    Id = s.Id,
                    Title = s.Title,
                    Slug = "/",
                    EditUrl = "manager/site/"
                }).ToList()
            };

            foreach (var site in model.Sites)
            {
                var sitemap = await _api.Sites.GetSitemapAsync(site.Id);

                foreach (var item in sitemap)
                {
                    site.Pages.Add(MapRecursive(item));
                }
            }

            return model;
        }

        private PageListModel.PageItem MapRecursive(SitemapItem item)
        {
            var model = new PageListModel.PageItem
            {
                Id = item.Id,
                Title = item.MenuTitle,
                TypeName = item.PageTypeName,
                Published = item.Published.HasValue ? item.Published.Value.ToString("yyyy-MM-dd") : null,
                EditUrl = "manager/page/"
            };

            foreach (var child in item.Items)
            {
                model.Items.Add(MapRecursive(child));
            }
            return model;
        }
    }
}