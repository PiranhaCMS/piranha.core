/*
 * Copyright (c) .NET Foundation and Contributors
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
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Piranha.Models;
using Piranha.Manager.Models;
using Piranha.Manager.Models.Content;
using Piranha.Services;

namespace Piranha.Manager.Services
{
    public class SiteService
    {
        private readonly IApi _api;
        private readonly IContentFactory _factory;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="factory">The content factory</param>
        public SiteService(IApi api, IContentFactory factory)
        {
            _api = api;
            _factory = factory;
        }

        /// <summary>
        /// Gets the edit model for the site with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The edit model</returns>
        public async Task<SiteEditModel> GetById(Guid id)
        {
            var site = await _api.Sites.GetByIdAsync(id);

            if (site != null)
            {
                return Transform(site);
            }
            return null;
        }

        /// <summary>
        /// Gets the content edit model for the site with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The edit model</returns>
        public async Task<SiteContentEditModel> GetContentById(Guid id)
        {
            var site = await _api.Sites.GetContentByIdAsync(id);

            if (site != null)
            {
                return Transform(site);
            }
            return null;
        }


        /// <summary>
        /// Creates a new site edit model.
        /// </summary>
        /// <returns>The edit model</returns>
        public SiteEditModel Create()
        {
            return new SiteEditModel
            {
                Id = Guid.NewGuid()
            };
        }

        /// <summary>
        /// Saves the given site.
        /// </summary>
        /// <param name="model">The site edit model</param>
        public async Task Save(SiteEditModel model)
        {
            var site = await _api.Sites.GetByIdAsync(model.Id);

            if (site == null)
            {
                site = new Site
                {
                    Id = model.Id
                };
            }
            site.SiteTypeId = model.TypeId;
            site.Title = model.Title;
            site.InternalId = model.InternalId;
            site.Culture = model.Culture;
            site.Hostnames = model.Hostnames;
            site.Description = model.Description;
            site.IsDefault = model.IsDefault;

            await _api.Sites.SaveAsync(site);
        }

        public async Task SaveContent(SiteContentEditModel model)
        {
            var siteType = App.SiteTypes.GetById(model.TypeId);

            if (siteType != null)
            {
                if (model.Id == Guid.Empty)
                {
                    model.Id = Guid.NewGuid();
                }

                var site = await _api.Sites.GetContentByIdAsync(model.Id);

                if (site == null)
                {
                    site = await _factory.CreateAsync<DynamicSiteContent>(siteType);
                    site.Id = model.Id;
                }

                site.TypeId = model.TypeId;
                site.Title = model.Title;

                // Save regions
                foreach (var region in siteType.Regions)
                {
                    var modelRegion = model.Regions
                        .FirstOrDefault(r => r.Meta.Id == region.Id);

                    if (region.Collection)
                    {
                        var listRegion = (IRegionList)((IDictionary<string, object>)site.Regions)[region.Id];

                        listRegion.Clear();

                        foreach (var item in modelRegion.Items)
                        {
                            if (region.Fields.Count == 1)
                            {
                                listRegion.Add(item.Fields[0].Model);
                            }
                            else
                            {
                                var postRegion = new ExpandoObject();

                                foreach (var field in region.Fields)
                                {
                                    var modelField = item.Fields
                                        .FirstOrDefault(f => f.Meta.Id == field.Id);
                                    ((IDictionary<string, object>)postRegion)[field.Id] = modelField.Model;
                                }
                                listRegion.Add(postRegion);
                            }
                        }
                    }
                    else
                    {
                        var postRegion = ((IDictionary<string, object>)site.Regions)[region.Id];

                        if (region.Fields.Count == 1)
                        {
                            ((IDictionary<string, object>)site.Regions)[region.Id] =
                                modelRegion.Items[0].Fields[0].Model;
                        }
                        else
                        {
                            foreach (var field in region.Fields)
                            {
                                var modelField = modelRegion.Items[0].Fields
                                    .FirstOrDefault(f => f.Meta.Id == field.Id);
                                ((IDictionary<string, object>)postRegion)[field.Id] = modelField.Model;
                            }
                        }
                    }
                }

                // Save site
                await _api.Sites.SaveContentAsync(model.Id, site);
            }
            else
            {
                throw new ValidationException("Invalid Post Type.");
            }
        }

        /// <summary>
        /// Deletes the site with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public Task Delete(Guid id)
        {
            return _api.Sites.DeleteAsync(id);
        }

        private SiteEditModel Transform(Site site)
        {
            return new SiteEditModel
            {
                Id = site.Id,
                TypeId = site.SiteTypeId,
                Title = site.Title,
                InternalId = site.InternalId,
                Culture = site.Culture,
                Description = site.Description,
                Hostnames = site.Hostnames,
                IsDefault = site.IsDefault,
                SiteTypes = App.SiteTypes.Select(t => new ContentTypeModel
                {
                    Id = t.Id,
                    Title = t.Title
                }).ToList()
            };
        }

        private SiteContentEditModel Transform(DynamicSiteContent site)
        {
            var type = App.SiteTypes.GetById(site.TypeId);

            var model = new SiteContentEditModel
            {
                Id = site.Id,
                TypeId = site.TypeId,
                Title = site.Title,
                UseBlocks = false
            };

            foreach (var regionType in type.Regions)
            {
                var region = new RegionModel
                {
                    Meta = new RegionMeta
                    {
                        Id = regionType.Id,
                        Name = regionType.Title,
                        Description = regionType.Description,
                        Placeholder = regionType.ListTitlePlaceholder,
                        IsCollection = regionType.Collection,
                        Expanded = regionType.ListExpand,
                        Icon = regionType.Icon,
                        Display = regionType.Display.ToString().ToLower()
                    }
                };
                var regionListModel = ((IDictionary<string, object>)site.Regions)[regionType.Id];

                if (!regionType.Collection)
                {
                    var regionModel = (IRegionList)Activator.CreateInstance(typeof(RegionList<>).MakeGenericType(regionListModel.GetType()));
                    regionModel.Add(regionListModel);
                    regionListModel = regionModel;
                }

                foreach (var regionModel in (IEnumerable)regionListModel)
                {
                    var regionItem = new RegionItemModel();

                    foreach (var fieldType in regionType.Fields)
                    {
                        var appFieldType = App.Fields.GetByType(fieldType.Type);

                        var field = new FieldModel
                        {
                            Meta = new FieldMeta
                            {
                                Id = fieldType.Id,
                                Name = fieldType.Title,
                                Component = appFieldType.Component,
                                Placeholder = fieldType.Placeholder,
                                IsHalfWidth = fieldType.Options.HasFlag(FieldOption.HalfWidth),
                                Description = fieldType.Description
                            }
                        };

                        if (typeof(Extend.Fields.SelectFieldBase).IsAssignableFrom(appFieldType.Type))
                        {
                            foreach(var item in ((Extend.Fields.SelectFieldBase)Activator.CreateInstance(appFieldType.Type)).Items)
                            {
                                field.Meta.Options.Add(Convert.ToInt32(item.Value), item.Title);
                            }
                        }

                        if (regionType.Fields.Count > 1)
                        {
                            field.Model = (Extend.IField)((IDictionary<string, object>)regionModel)[fieldType.Id];

                            if (regionType.ListTitleField == fieldType.Id)
                            {
                                regionItem.Title = field.Model.GetTitle();
                                field.Meta.NotifyChange = true;
                            }
                        }
                        else
                        {
                            field.Model = (Extend.IField)regionModel;
                            field.Meta.NotifyChange = true;
                            regionItem.Title = field.Model.GetTitle();
                        }
                        regionItem.Fields.Add(field);
                    }

                    if (string.IsNullOrWhiteSpace(regionItem.Title))
                    {
                        regionItem.Title = "...";
                    }

                    region.Items.Add(regionItem);
                }
                model.Regions.Add(region);
            }
            return model;
        }
    }
}