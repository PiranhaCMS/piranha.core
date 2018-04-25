/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Piranha.Services;
using System.Collections.Generic;

namespace Piranha.Models
{
    /// <summary>
    /// Generic page model.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    public class GenericPage<T> : PageBase where T : GenericPage<T>
    {
        /// <summary>
        /// Gets if this is the startpage of the site.
        /// </summary>
        public bool IsStartPage {
            get { return !ParentId.HasValue && SortOrder == 0; }
        }

        /// <summary>
        /// Gets/sets the available blocks.
        /// </summary>
        public IList<Extend.Block> Blocks { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GenericPage() : base() {
            Blocks = new List<Extend.Block>();
        }

        /// <summary>
        /// Creates a new page model using the given page type id.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="typeId">The unique page type id</param>
        /// <returns>The new model</returns>
        public static T Create(IApi api, string typeId = null) {
            return api.Pages.Create<T>(typeId);
        }

        /// <summary>
        /// Creates a new region.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="typeId">The page type id</param>
        /// <param name="regionId">The region id</param>
        /// <returns>The new region value</returns>
        public static object CreateRegion(IApi api, string typeId, string regionId) {
            using (var factory = new ContentFactory(api.PageTypes.GetAll())) {
                return factory.CreateDynamicRegion(typeId, regionId);
            }
        }

        /// <summary>
        /// Creates a new region for the current model.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="regionId">The region id</param>
        /// <returns>The new region value</returns>
        public object CreateRegion(IApi api, string regionId) {
            return CreateRegion(api, TypeId, regionId);
        }
    }
}
