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
using System.Collections;
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
                var type = App.PageTypes.GetById(page.TypeId);

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

                foreach (var regionType in type.Regions)
                {
                    var region = new RegionEditModel
                    {
                        Meta = new ContentRegionMeta
                        {
                            Name = regionType.Title,
                            Description = regionType.Description,
                            Placeholder = regionType.ListTitlePlaceholder,
                            IsCollection = regionType.Collection,
                            Icon = regionType.Icon,
                            Display = regionType.Display.ToString().ToLower()
                        }
                    };
                    var regionListModel = ((IDictionary<string, object>)page.Regions)[regionType.Id];

                    if (!regionType.Collection)
                    {
                        var regionModel = (IRegionList)Activator.CreateInstance(typeof(RegionList<>).MakeGenericType(regionListModel.GetType()));
                        regionModel.Add(regionListModel);
                        regionListModel = regionModel;
                    }

                    foreach (var regionModel in (IEnumerable)regionListModel)
                    {
                        var regionItem = new RegionItemEditModel();

                        foreach (var fieldType in regionType.Fields)
                        {
                            var appFieldType = App.Fields.GetByType(fieldType.Type);

                            var field = new FieldEditModel
                            {
                                Type = appFieldType.TypeName,
                                Meta = new ContentFieldMeta
                                {
                                    Name = fieldType.Title,
                                    Component = appFieldType.Component,
                                    Placeholder = fieldType.Placeholder,
                                    IsHalfWidth = fieldType.Options.HasFlag(FieldOption.HalfWidth)
                                }
                            };

                            if (regionType.Fields.Count > 1)
                            {
                                field.Model = (Extend.IField)((IDictionary<string, object>)regionModel)[fieldType.Id];
                            }
                            else
                            {
                                field.Model = (Extend.IField)regionModel;
                            }
                            regionItem.Fields.Add(field);
                        }
                        region.Items.Add(regionItem);
                    }
                    model.Regions.Add(region);
                }

                foreach (var block in page.Blocks)
                {
                    var blockType = App.Blocks.GetByType(block.Type);

                    if (block is Extend.BlockGroup)
                    {
                        var group = new BlockEditModel
                        {
                            Meta = new ContentMeta
                            {
                                Name = blockType.Name,
                                Icon = blockType.Icon,
                                Component = "block-group"
                            }
                        };

                        var groupItem = new BlockGroupEditModel
                        {
                            Type = block.Type
                        };

                        foreach (var prop in block.GetType().GetProperties(App.PropertyBindings))
                        {
                            if (typeof(Extend.IField).IsAssignableFrom(prop.PropertyType))
                            {
                                var fieldType = App.Fields.GetByType(prop.PropertyType);

                                groupItem.Fields.Add(new FieldEditModel
                                {
                                    Type = fieldType.TypeName,
                                    Model = (Extend.IField)prop.GetValue(block),
                                    Meta = new ContentFieldMeta
                                    {
                                        Name = prop.Name,
                                        Component = fieldType.Component,
                                    }
                                });
                            }
                        }

                        bool firstChild = true;
                        foreach (var child in ((Extend.BlockGroup)block).Items)
                        {
                            blockType = App.Blocks.GetByType(child.Type);

                            groupItem.Items.Add(new BlockEditModel
                            {
                                IsActive = firstChild,
                                Model = child,
                                Meta = new ContentMeta
                                {
                                    Name = blockType.Name,
                                    Title = child.GetTitle(),
                                    Icon = blockType.Icon,
                                    Component = blockType.Component
                                }
                            });
                            firstChild = false;
                        }
                        group.Model = groupItem;
                        model.Blocks.Add(group);
                    }
                    else
                    {
                        model.Blocks.Add(new BlockEditModel
                        {
                            Model = block,
                            Meta = new ContentMeta
                            {
                                Name = blockType.Name,
                                Title = block.GetTitle(),
                                Icon = blockType.Icon,
                                Component = blockType.Component
                            }
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