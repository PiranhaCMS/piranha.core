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
using System.Dynamic;

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
    public class PageModel<T> : PageModelBase where T : PageModel<T>
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
            using (var factory = new ContentFactory(App.PageTypes)) {
                return factory.Create<T>(typeId);
            }
        }

        /// <summary>
        /// Creates a new region.
        /// </summary>
        /// <param name="typeId">The page type id</param>
        /// <param name="regionId">The region id</param>
        /// <returns>The new region value</returns>
        public static object CreateRegion(string typeId, string regionId) {
            using (var factory = new ContentFactory(App.PageTypes)) {
                return factory.CreateRegion(typeId, regionId);
            }
        }
    }

    /// <summary>
    /// Base class for page models.
    /// </summary>
    public abstract class PageModelBase : Content
    {
        #region Properties
        /// <summary>
        /// Gets/sets the optional parent id.
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Gets/sets the sort order of the page in its hierarchical position.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets/sets the navigation title.
        /// </summary>
        public string NavigationTitle { get; set; }

        /// <summary>
        /// Gets/sets the unique slug.
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Gets/sets the optional meta keywords.
        /// </summary>
        public string MetaKeywords { get; set; }

        /// <summary>
        /// Gets/sets the optional meta description.
        /// </summary>
        public string MetaDescription { get; set; }

        /// <summary>
        /// Gets/sets the internal route used by the middleware.
        /// </summary>
        public string Route { get; set; }
        #endregion
    }
}
