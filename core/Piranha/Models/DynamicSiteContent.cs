/*
 * Copyright (c) 2016-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Dynamic;

namespace Piranha.Models
{
    /// <summary>
    /// Dynamic page model.
    /// </summary>
    public class DynamicSiteContent : SiteContent<DynamicSiteContent>, IDynamicModel
    {
        /// <summary>
        /// Gets/sets the regions.
        /// </summary>
        public dynamic Regions { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DynamicSiteContent() : base()
        {
            Regions = new ExpandoObject();
        }

        /// <summary>
        /// Creates a new region.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="typeId">The page type id</param>
        /// <param name="regionId">The region id</param>
        /// <returns>The new region value</returns>
        public static object CreateRegion(IApi api, string typeId, string regionId)
        {
            using (var factory = new ContentFactory(api.SiteTypes.GetAll()))
            {
                return factory.CreateDynamicRegion(typeId, regionId);
            }
        }

        /// <summary>
        /// Creates a new region for the current model.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="regionId">The region id</param>
        /// <returns>The new region value</returns>
        public object CreateRegion(IApi api, string regionId)
        {
            return CreateRegion(api, TypeId, regionId);
        }        
    }
}