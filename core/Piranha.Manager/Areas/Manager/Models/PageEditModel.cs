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
using Piranha.Manager;

namespace Piranha.Areas.Manager.Models
{
    /// <summary>
    /// The page edit view model.
    /// </summary>
    public class PageEditModel : Piranha.Models.PageBase
    {
        #region Properties
        /// <summary>
        /// Gets/sets the page type.
        /// </summary>
        public Piranha.Models.PageType PageType { get; set; }

        /// <summary>
        /// Gets/sets the available regions.
        /// </summary>
        public IList<PageEditRegionBase> Regions { get; set; }
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PageEditModel() {
            Regions = new List<PageEditRegionBase>();
        }

        /// <summary>
        /// Saves the page model.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="publish">If the page should be published</param>
        /// <returns>If the page was successfully saved</returns>
        public bool Save(IApi api, bool? publish = null) {
            var page = api.Pages.GetById(Id);

            if (page == null)
                page = Piranha.Models.DynamicPage.Create(api, this.TypeId);

            Module.Mapper.Map<PageEditModel, Piranha.Models.PageBase>(this, page);
            SaveRegions(api, this, page);

            if (publish.HasValue) {
                if (publish.Value && !page.Published.HasValue)
                    page.Published = DateTime.Now;
                else if (!publish.Value)
                    page.Published = null;
            }
            api.Pages.Save(page);
            Id = page.Id;

            return true;
        }

        /// <summary>
        /// Gets the edit model for the page with the given id.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="id">The page id</param>
        /// <returns>The page model</returns>
        public static PageEditModel GetById(IApi api, string id) {
            var page = api.Pages.GetById(id);
            if (page != null) {
                var model = Module.Mapper.Map<Piranha.Models.PageBase, PageEditModel>(page);
                model.PageType = api.PageTypes.GetById(model.TypeId);
                LoadRegions(page, model);

                return model;
            }
            throw new KeyNotFoundException($"No page found with the id '{id}'");
        }

        /// <summary>
        /// Refreshes the model after an unsuccessful save.
        /// </summary>
        public PageEditModel Refresh(IApi api) {
            if (!string.IsNullOrWhiteSpace(TypeId))
                PageType = api.PageTypes.GetById(TypeId);
            return this;
        }

        /// <summary>
        /// Creates a new edit model with the given page typeparamref.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="pageTypeId">The page type id</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The page model</returns>        
        public static PageEditModel Create(IApi api, string pageTypeId, string siteId = null) {
            var type = api.PageTypes.GetById(pageTypeId);

            if (string.IsNullOrEmpty(siteId)) {
                var site = api.Sites.GetDefault();

                if (site != null)
                    siteId = site.Id;
            }

            if (type != null) {
                var page = Piranha.Models.DynamicPage.Create(api, pageTypeId);
                var model = Module.Mapper.Map<Piranha.Models.PageBase, PageEditModel>(page);
                model.SiteId = siteId;
                model.PageType = type;
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
        public static PageEditRegionBase CreateRegion(Piranha.Models.RegionType region, object value) {
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
                                Value = (Extend.IField)fieldData[field.Id]
                            });
                        }
                    }
                    editRegion.Add(fieldSet);
                }
            }
            return editRegion;
        }

        #region Private methods
        /// <summary>
        /// Loads all of the regions from the source model into the destination.
        /// </summary>
        /// <param name="src">The source</param>
        /// <param name="dest">The destination</param>
        private static void LoadRegions(Piranha.Models.DynamicPage src, PageEditModel dest) {
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
        /// Saves all of the regions from the source model into the destination.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="src">The source</param>
        /// <param name="dest">The destination</param>
        private static void SaveRegions(IApi api, PageEditModel src, Piranha.Models.DynamicPage dest) {
            var modelRegions = (IDictionary<string, object>)dest.Regions;
            foreach (var region in src.Regions) {
                if (region is PageEditRegion) {
                    if (!modelRegions.ContainsKey(region.Id))
                        modelRegions[region.Id] = Piranha.Models.DynamicPage.CreateRegion(api, dest.TypeId, region.Id);

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
                                var modelFields = (IDictionary<string, object>)Piranha.Models.DynamicPage.CreateRegion(api, dest.TypeId, region.Id);

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
        #endregion
    }
}
