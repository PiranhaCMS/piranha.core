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

        public async Task<PageEditModel> GetById(Guid id)
        {
            var page = await _api.Pages.GetByIdAsync(id);

            if (page != null)
            {
                var model = new PageEditModel
                {
                    Id = page.Id,
                    SiteId = page.SiteId,
                    ParentId = page.ParentId,
                    SortOrder = page.SortOrder,
                    TypeId = page.TypeId,
                    Title = page.Title,
                    NavigationTitle = page.NavigationTitle,
                    Slug = page.Slug,
                    MetaKeywords = page.MetaKeywords,
                    MetaDescription = page.MetaDescription,
                    Published = page.Published.HasValue ? page.Published.Value.ToString("yyyy-MM-dd HH:mm") : null
                };

                foreach (var block in page.Blocks)
                {
                    var blockType = App.Blocks.GetByType(block.Type);

                    if (block is Extend.BlockGroup)
                    {
                        var group = new ContentEditModel.BlockItem
                        {
                            Name = blockType.Name,
                            Icon = blockType.Icon,
                            Component = "block-group"
                        };

                        var groupItem = new ContentEditModel.BlockGroupItem
                        {
                            Type = block.Type
                        };

                        foreach (var prop in block.GetType().GetProperties(App.PropertyBindings))
                        {
                            if (typeof(Extend.IField).IsAssignableFrom(prop.PropertyType))
                            {
                                var fieldType = App.Fields.GetByType(prop.PropertyType);

                                groupItem.Fields.Add(new ContentEditModel.FieldItem
                                {
                                    Name = prop.Name,
                                    Type = fieldType.TypeName,
                                    Component = fieldType.Component,
                                    Model = (Extend.IField)prop.GetValue(block)
                                });
                            }
                        }

                        bool firstChild = true;
                        foreach (var child in ((Extend.BlockGroup)block).Items)
                        {
                            blockType = App.Blocks.GetByType(child.Type);

                            groupItem.Items.Add(new ContentEditModel.BlockItem
                            {
                                Name = blockType.Name,
                                Icon = blockType.Icon,
                                Component = blockType.Component,
                                IsActive = firstChild,
                                Title = child.GetTitle(),
                                Model = child
                            });
                            firstChild = false;
                        }
                        group.Model = groupItem;
                        model.Blocks.Add(group);
                    }
                    else
                    {
                        model.Blocks.Add(new ContentEditModel.BlockItem
                        {
                            Name = blockType.Name,
                            Icon = blockType.Icon,
                            Component = blockType.Component,
                            Title = block.GetTitle(),
                            Model = block
                        });
                    }
                }
                return model;
            }
            return null;
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