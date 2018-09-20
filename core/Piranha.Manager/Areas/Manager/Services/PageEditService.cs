/*
 * Copyright (c) 2016-2017 Håkan Edling
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
using Piranha.Areas.Manager.Models;
using Piranha.Manager;
using Piranha.Services;

namespace Piranha.Areas.Manager.Services
{
    /// <summary>
    /// The page edit service.
    /// </summary>
    public class PageEditService
    {
        private readonly IApi api;
        private readonly IContentService<Data.Page, Data.PageField, Piranha.Models.PageBase> service;

        public PageEditService(IApi api, IContentServiceFactory factory) {
            this.api = api;
            service = factory.CreatePageService();
        }

        /// <summary>
        /// Saves the page model.
        /// </summary>
        /// <param name="model">The page edit model</param>
        /// <param name="alias">The suggested alias</param>
        /// <param name="publish">If the page should be published</param>
        /// <returns>If the page was successfully saved</returns>
        public bool Save(PageEditModel model, out string alias, bool? publish = null) {
            var page = api.Pages.GetById(model.Id);
            alias = null;

            if (page == null) {
                page = Piranha.Models.DynamicPage.Create(api, model.TypeId);
            } else {
                if (model.Slug != page.Slug && model.Published.HasValue)
                    alias = page.Slug;
            }

            Module.Mapper.Map<PageEditModel, Piranha.Models.PageBase>(model, page);
            SaveRegions(model, page);
            SaveBlocks(model, page);

            if (publish.HasValue) {
                if (publish.Value && !page.Published.HasValue)
                    page.Published = DateTime.Now;
                else if (!publish.Value)
                    page.Published = null;
            }
            api.Pages.Save(page);
            model.Id = page.Id;

            return true;
        }

        /// <summary>
        /// Gets the edit model for the page with the given id.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="id">The page id</param>
        /// <returns>The page model</returns>
        public PageEditModel GetById(Guid id) {
            var page = api.Pages.GetById(id);
            if (page != null) {
                var model = Module.Mapper.Map<Piranha.Models.PageBase, PageEditModel>(page);
                model.PageType = api.PageTypes.GetById(model.TypeId);
                model.PageContentType = App.ContentTypes.GetById(model.PageType.ContentTypeId);
                LoadRegions(page, model);
                LoadBlocks(page, model);

                return model;
            }
            throw new KeyNotFoundException($"No page found with the id '{id}'");
        }

        /// <summary>
        /// Refreshes the model after an unsuccessful save.
        /// </summary>
        public PageEditModel Refresh(PageEditModel model) {
            if (!string.IsNullOrWhiteSpace(model.TypeId)) {
                model.PageType = api.PageTypes.GetById(model.TypeId);
                model.PageContentType = App.ContentTypes.GetById(model.PageType.ContentTypeId);
            }
            return model;
        }

        /// <summary>
        /// Creates a new edit model with the given page typeparamref.
        /// </summary>
        /// <param name="pageTypeId">The page type id</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The page model</returns>        
        public PageEditModel Create(string pageTypeId, Guid? siteId = null) {
            var type = api.PageTypes.GetById(pageTypeId);

            if (!siteId.HasValue) {
                var site = api.Sites.GetDefault();

                if (site != null)
                    siteId = site.Id;
            }

            if (type != null) {
                var page = Piranha.Models.DynamicPage.Create(api, pageTypeId);
                var model = Module.Mapper.Map<Piranha.Models.PageBase, PageEditModel>(page);
                model.SiteId = siteId.Value;
                model.PageType = type;
                model.PageContentType = App.ContentTypes.GetById(type.ContentTypeId);
                model.ContentType = model.PageContentType != null ? model.PageContentType.Id : null;

                LoadRegions(page, model);

                return model;
            }
            throw new KeyNotFoundException($"No page type found with the id '{pageTypeId}'");
        }

        /// <summary>
        /// Creates a new edit region model for the given region type and value.
        /// </summary>
        /// <param name="region">The region type</param>
        /// <param name="value">The region value</param>
        /// <returns>The edit model</returns>
        public PageEditRegionBase CreateRegion(Piranha.Models.RegionType region, object value) {
            PageEditRegionBase editRegion;

            if (region.Collection) {
                editRegion = new PageEditRegionCollection();
            } else {
                editRegion = new PageEditRegion();
            }
            editRegion.Id = region.Id;
            editRegion.Title = region.Title ?? region.Id;
            editRegion.CLRType = editRegion.GetType().FullName;

            IList items = new List<object>();

            if (region.Collection)
                items = (IList)value;
            else items.Add(value);

            foreach (var item in items) {
                if (region.Fields.Count == 1) {
                    var itemTitle = "";

                    // Get the item title if this is a collection region.
                    if (region.Collection) {
                        if (item != null)
                            itemTitle = ((Extend.IField)item).GetTitle();
                        if (string.IsNullOrWhiteSpace(itemTitle) && !string.IsNullOrWhiteSpace(region.ListTitlePlaceholder))
                            itemTitle = region.ListTitlePlaceholder;
                        else itemTitle = "Item";
                    }

                    var set = new PageEditFieldSet() {
                        new PageEditField() {
                            Id = region.Fields[0].Id,
                            Title = region.Fields[0].Title ?? region.Fields[0].Id,
                            CLRType = item.GetType().FullName,
                            Options = region.Fields[0].Options,
                            Placeholder = region.Fields[0].Placeholder,
                            Value = (Extend.IField)item
                        }
                    };
                    set.ListTitle = itemTitle;
                    set.NoExpand = !region.ListExpand;

                    editRegion.Add(set);
                } else {
                    var fieldData = (IDictionary<string, object>)item;
                    var fieldSet = new PageEditFieldSet();

                    foreach (var field in region.Fields) {
                        if (fieldData.ContainsKey(field.Id)) {
                            // Get the item title if this is a collection region.
                            if (region.Collection) {
                                if (!string.IsNullOrWhiteSpace(region.ListTitleField) && field.Id == region.ListTitleField) {
                                    var itemTitle = "";

                                    if (fieldData[field.Id] != null)
                                        itemTitle = ((Extend.IField)fieldData[field.Id]).GetTitle();
                                    if (string.IsNullOrWhiteSpace(itemTitle) && !string.IsNullOrWhiteSpace(region.ListTitlePlaceholder))
                                        itemTitle = region.ListTitlePlaceholder;
                                    else if (string.IsNullOrWhiteSpace(itemTitle)) 
                                        itemTitle = "Item";

                                    fieldSet.ListTitle = itemTitle;
                                    fieldSet.NoExpand = !region.ListExpand;
                                }
                            }

                            fieldSet.Add(new PageEditField() {
                                Id = field.Id,
                                Title = field.Title ?? field.Id,
                                CLRType = fieldData[field.Id].GetType().FullName,
                                Options = field.Options,
                                Placeholder = field.Placeholder,
                                Value = (Extend.IField)fieldData[field.Id]
                            });
                        }
                    }
                    editRegion.Add(fieldSet);
                }
            }
            return editRegion;
        }

        /// <summary>
        /// Loads all of the regions from the source model into the destination.
        /// </summary>
        /// <param name="src">The source</param>
        /// <param name="dest">The destination</param>
        private void LoadRegions(Piranha.Models.DynamicPage src, PageEditModel dest) {
            if (dest.PageType != null) {
                foreach (var region in dest.PageType.Regions) {
                    var regions = (IDictionary<string, object>)src.Regions;

                    if (regions.ContainsKey(region.Id)) {
                        var editRegion = CreateRegion(region, regions[region.Id]);
                        dest.Regions.Add(editRegion);
                    }
                }
            }
        }

        /// <summary>
        /// Loads the available blocks from the source model into the destination.
        /// </summary>
        /// <param name="src">The source</param>
        /// <param name="dest">The destination</param>
        private void LoadBlocks(Piranha.Models.DynamicPage src, PageEditModel dest) {
            foreach (var srcBlock in src.Blocks) {
                var block = new ContentEditBlock() {
                    Id = srcBlock.Id,
                    CLRType = srcBlock.GetType().FullName,
                    IsGroup = typeof(Extend.BlockGroup).IsAssignableFrom(srcBlock.GetType()),
                    Value = srcBlock
                };

                if (typeof(Extend.BlockGroup).IsAssignableFrom(srcBlock.GetType())) {
                    foreach (var subBlock in ((Extend.BlockGroup)srcBlock).Items) {
                        block.Items.Add(new ContentEditBlock() {
                            Id = subBlock.Id,
                            CLRType = subBlock.GetType().FullName,
                            Value = subBlock
                        });
                    }
                }
                dest.Blocks.Add(block);
            }
        }

        private void SaveBlocks(PageEditModel src, Piranha.Models.DynamicPage dest) {
            // Clear the block list
            dest.Blocks.Clear();

            // And rebuild it
            foreach (var srcBlock in src.Blocks) {
                dest.Blocks.Add(srcBlock.Value);

                if (typeof(Extend.BlockGroup).IsAssignableFrom(srcBlock.Value.GetType())) {
                    foreach (var subBlock in srcBlock.Items)
                        ((Extend.BlockGroup)srcBlock.Value).Items.Add(subBlock.Value);
                }
            }
        }

        /// <summary>
        /// Saves all of the regions from the source model into the destination.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="src">The source</param>
        /// <param name="dest">The destination</param>
        private void SaveRegions(PageEditModel src, Piranha.Models.DynamicPage dest) {
            var type = api.PageTypes.GetById(src.TypeId);
            var modelRegions = (IDictionary<string, object>)dest.Regions;

            foreach (var region in src.Regions) {
                if (region is PageEditRegion) {
                    if (!modelRegions.ContainsKey(region.Id))
                        modelRegions[region.Id] = service.CreateDynamicRegion(type, region.Id);

                    var reg = (PageEditRegion)region;

                    if (reg.FieldSet.Count == 1) {
                        modelRegions[region.Id] = reg.FieldSet[0].Value;
                    } else {
                        var modelFields = (IDictionary<string, object>)modelRegions[region.Id];

                        foreach (var field in reg.FieldSet) {
                            modelFields[field.Id] = field.Value;
                        }
                    }
                } else {
                    if (modelRegions.ContainsKey(region.Id)) {
                        var list = (Piranha.Models.IRegionList)modelRegions[region.Id];
                        var reg = (PageEditRegionCollection)region;

                        // At this point we clear the values and rebuild them
                        list.Clear();

                        foreach (var set in reg.FieldSets) {
                            if (set.Count == 1) {
                                list.Add(set[0].Value);
                            } else {
                                var modelFields = (IDictionary<string, object>)service.CreateDynamicRegion(type, region.Id);

                                foreach (var field in set) {
                                    modelFields[field.Id] = field.Value;
                                }
                                list.Add(modelFields);
                            }
                        }
                    }
                }
            }
        }
    }
}
