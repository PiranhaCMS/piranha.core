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
using Piranha.Areas.Manager.Models;
using Piranha.Manager;
using Piranha.Services;

namespace Piranha.Areas.Manager.Services
{
    /// <summary>
    /// The page edit service.
    /// </summary>
    public class SiteContentEditService
    {
        private readonly IApi api;
        private readonly IContentService<Data.Site, Data.SiteField, Piranha.Models.SiteContentBase> service;

        public SiteContentEditService(IApi api, IContentServiceFactory factory) {
            this.api = api;
            service = factory.CreateSiteService();
        }

        /// <summary>
        /// Saves the site content model.
        /// </summary>
        /// <param name="model">The site content edit model</param>
        /// <returns>If the model was successfully saved</returns>
        public bool Save(SiteContentEditModel model) {
            var site = api.Sites.GetContentById(model.Id);

            if (site == null) {
                site = Piranha.Models.DynamicSiteContent.Create(api, model.TypeId);
            }

            Module.Mapper.Map<SiteContentEditModel, Piranha.Models.SiteContentBase>(model, site);
            SaveRegions(model, site);

            api.Sites.SaveContent(model.Id, site);
            model.Id = site.Id;

            return true;
        }

        /// <summary>
        /// Gets the edit model for the post with the given id.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="id">The post id</param>
        /// <returns>The post model</returns>
        public SiteContentEditModel GetById(Guid id) {
            var site = api.Sites.GetContentById(id);

            if (site == null) {
                var siteInfo = api.Sites.GetById(id);
                site = Piranha.Models.DynamicSiteContent.Create(api, siteInfo.SiteTypeId);

                site.Id = siteInfo.Id;
                site.TypeId = siteInfo.SiteTypeId;
                site.Title = siteInfo.Title;
            }

            if (site != null) {
                var model = Module.Mapper.Map<Piranha.Models.SiteContentBase, SiteContentEditModel>(site);
                model.SiteType = api.SiteTypes.GetById(model.TypeId);

                LoadRegions(site, model);

                return model;
            }
            throw new KeyNotFoundException($"No site found with the id '{id}'");
        }

        /// <summary>
        /// Refreshes the model after an unsuccessful save.
        /// </summary>
        public SiteContentEditModel Refresh(SiteContentEditModel model) {
            if (!string.IsNullOrWhiteSpace(model.TypeId)) {
                model.SiteType = api.SiteTypes.GetById(model.TypeId);
            }
            return model;
        }

        /// <summary>
        /// Creates a new edit model with the given site typeparamref.
        /// </summary>
        /// <param name="siteTypeId">The site type id</param>
        /// <returns>The site content edit model</returns>        
        public SiteContentEditModel Create(string siteTypeId) {
            var type = api.SiteTypes.GetById(siteTypeId);

            if (type != null) {
                var site = Piranha.Models.DynamicSiteContent.Create(api, siteTypeId);
                var model = Module.Mapper.Map<Piranha.Models.SiteContentBase, SiteContentEditModel>(site);
                model.SiteType = type;

                LoadRegions(site, model);

                return model;
            }
            throw new KeyNotFoundException($"No site type found with the id '{siteTypeId}'");
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

        /// <summary>
        /// Loads all of the regions from the source model into the destination.
        /// </summary>
        /// <param name="src">The source</param>
        /// <param name="dest">The destination</param>
        private void LoadRegions(Piranha.Models.DynamicSiteContent src, SiteContentEditModel dest) {
            if (dest.SiteType != null) {
                foreach (var region in dest.SiteType.Regions) {
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
        /// <param name="src">The source</param>
        /// <param name="dest">The destination</param>
        private void SaveRegions(SiteContentEditModel src, Piranha.Models.DynamicSiteContent dest) {
            var type = api.SiteTypes.GetById(src.TypeId);
            var modelRegions = (IDictionary<string, object>)dest.Regions;
            
            foreach (var region in src.Regions) {
                if (region is PageEditRegion) {
                    if (!modelRegions.ContainsKey(region.Id))
                        modelRegions[region.Id] = service.CreateDynamicRegion(type, region.Id);
                        //modelRegions[region.Id] = Piranha.Models.DynamicSiteContent.CreateRegion(api, dest.TypeId, region.Id);

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
