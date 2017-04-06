/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha.Models
{
    /// <summary>
    /// Generic page model.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    public class Page<T> : PageBase where T : Page<T>
    {
        #region Properties
        /// <summary>
        /// Gets if this is the startpage of the site.
        /// </summary>
        public bool IsStartPage {
            get { return string.IsNullOrEmpty(ParentId) && SortOrder == 0; }
        }
        #endregion

        /// <summary>
        /// Creates a new page model using the given page type id.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="typeId">The unique page type id</param>
        /// <returns>The new model</returns>
        public static T Create(Api api, string typeId = null) {
            if (string.IsNullOrWhiteSpace(typeId))
                typeId = typeof(T).Name;

            using (var factory = new ContentFactory(api.PageTypes.GetAll())) {
                return factory.Create<T>(typeId);
            }
        }

        /// <summary>
        /// Creates a new region.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="typeId">The page type id</param>
        /// <param name="regionId">The region id</param>
        /// <returns>The new region value</returns>
        public static object CreateRegion(Api api, string typeId, string regionId) {
            using (var factory = new ContentFactory(api.PageTypes.GetAll())) {
                return factory.CreateDynamicRegion(typeId, regionId);
            }
        }
    }
}
