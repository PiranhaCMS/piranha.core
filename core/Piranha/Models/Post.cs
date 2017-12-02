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
    /// Generic post model.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    public class Post<T> : PostBase where T : Post<T>
    {
        /// <summary>
        /// Creates a new post model using the given post type id.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="typeId">The unique post type id</param>
        /// <returns>The new model</returns>
        public static T Create(IApi api, string typeId = null) {
            if (string.IsNullOrWhiteSpace(typeId))
                typeId = typeof(T).Name;

            using (var factory = new ContentFactory(api.PostTypes.GetAll())) {
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
        public static object CreateRegion(IApi api, string typeId, string regionId) {
            using (var factory = new ContentFactory(api.PostTypes.GetAll())) {
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
