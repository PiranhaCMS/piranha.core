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
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Piranha.Models
{
    /// <summary>
    /// Dynamic page model.
    /// </summary>
    public class PageModel : PageModel<PageModel>
    {
        #region Properties
        /// <summary>
        /// Gets/sets the regions.
        /// </summary>
        public dynamic Regions { get; set; }
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PageModel() : base() {
            Regions = new ExpandoObject();
        }
    }

    /// <summary>
    /// Generic page model.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    public class PageModel<T> : PageBase where T : PageModel<T>
    {
        #region Properties
        /// <summary>
        /// Gets if this is the startpage of the site.
        /// </summary>
        public bool IsStartPage {
            get { return !ParentId.HasValue && SortOrder == 0; }
        }
        #endregion

        /// <summary>
        /// Creates a new page model using the given page type id.
        /// </summary>
        /// <param name="typeId">The unique page type id</param>
        /// <returns>The new model</returns>
        public static T Create(string typeId) {
            var pageType = App.PageTypes
                .SingleOrDefault(t => t.Id == typeId);

            if (pageType != null) {
                var model = (T)Activator.CreateInstance<T>();
                model.PageTypeId = typeId;

                if (model is PageModel) {
                    var dynModel = (PageModel)(object)model;

                    foreach (var region in pageType.Regions) {
                        object value = null;

                        if (region.Collection) {
                            var reg = CreateRegion(region);

                            if (reg != null) {
                                value = Activator.CreateInstance(typeof(PageRegionList<>).MakeGenericType(reg.GetType()));
                                ((IPageRegionList)value).TypeId = typeId;
                                ((IPageRegionList)value).RegionId = region.Id;
                            }
                        } else {
                            value = CreateRegion(region);
                        }

                        if (value != null)
                            ((IDictionary<string, object>)dynModel.Regions).Add(region.Id, value);
                    }
                } else {
                    var typeInfo = model.GetType().GetTypeInfo();

                    foreach (var region in pageType.Regions) {
                        if (!region.Collection) {
                            var prop = typeInfo.GetProperty(region.Id, App.PropertyBindings);
                            if (prop != null) {
                                prop.SetValue(model, CreateRegion(prop.PropertyType, region));
                            }
                        }
                    }
                }
                return model;
            }
            return null;
        }

        /// <summary>
        /// Creates a new region.
        /// </summary>
        /// <param name="typeId">The page type id</param>
        /// <param name="regionId">The region id</param>
        /// <returns>The new region value</returns>
        public static object CreateRegion(string typeId, string regionId) {
            var pageType = App.PageTypes
                .SingleOrDefault(t => t.Id == typeId);

            if (pageType != null) {
                var region = pageType.Regions.SingleOrDefault(r => r.Id == regionId);

                if (region != null)
                    return CreateRegion(region);
            }
            return null;
        }

        /// <summary>
        /// Creates a dynamic region.
        /// </summary>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <param name="typeId">The page type id</param>
        /// <param name="regionId">The region id</param>
        /// <returns>The region value</returns>
        public static TValue CreateRegion<TValue>(string typeId, string regionId) {
            var pageType = App.PageTypes
                .SingleOrDefault(t => t.Id == typeId);

            if (pageType != null) {
                var region = pageType.Regions.SingleOrDefault(r => r.Id == regionId);

                if (region != null)
                    return (TValue)CreateRegion(typeof(TValue), region);
            }
            return default(TValue);
        }

        #region Private methods
        /// <summary>
        /// Creates a region value.
        /// </summary>
        /// <param name="region">The region type</param>
        /// <returns>The created value</returns>
        private static object CreateRegion(PageTypeRegion region) {
            if (region.Fields.Count == 1) {
                var type = App.Fields.GetByShorthand(region.Fields[0].Type);
                if (type == null)
                    type = App.Fields.GetByType(region.Fields[0].Type);

                if (type != null)
                    return Activator.CreateInstance(type.Type);
            } else {
                var reg = new ExpandoObject();

                foreach (var field in region.Fields) {
                    var type = App.Fields.GetByShorthand(field.Type);
                    if (type == null)
                        type = App.Fields.GetByType(field.Type);

                    if (type != null)
                        ((IDictionary<string, object>)reg).Add(field.Id, Activator.CreateInstance(type.Type));
                }
                return reg;
            }
            return null;
        }

        /// <summary>
        /// Creates a region value.
        /// </summary>
        /// <param name="regionType">The regio ntype</param>
        /// <param name="region">The region</param>
        /// <returns>The created value</returns>
        private static object CreateRegion(Type regionType, PageTypeRegion region) {
            if (region.Fields.Count == 1) {
                var type = App.Fields.GetByShorthand(region.Fields[0].Type);
                if (type == null)
                    type = App.Fields.GetByType(region.Fields[0].Type);

                if (type != null && type.Type == regionType)
                    return Activator.CreateInstance(type.Type);
                return null;
            } else {
                var reg = Activator.CreateInstance(regionType);
                var typeInfo = reg.GetType().GetTypeInfo();

                foreach (var field in region.Fields) {
                    var type = App.Fields.GetByShorthand(field.Type);
                    if (type == null)
                        type = App.Fields.GetByType(field.Type);

                    if (type != null) {
                        var prop = typeInfo.GetProperty(field.Id, App.PropertyBindings);

                        if (prop != null && type.Type == prop.PropertyType) {
                            prop.SetValue(reg, Activator.CreateInstance(type.Type));
                        }
                    }
                }
                return reg;
            }
        }
        #endregion
    }
}
