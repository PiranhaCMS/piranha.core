/*
 * Copyright (c) 2016 Håkan Edling
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
    /// Generic block model.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    public class Block<T> : BlockBase where T : Block<T>
    {
        /// <summary>
        /// Creates a new block model using the given block type id.
        /// </summary>
        /// <param name="typeId">The unique block type id</param>
        /// <returns>The new model</returns>
        public static T Create(string typeId) {
            using (var factory = new ContentFactory(App.BlockTypes)) {
                return factory.Create<T>(typeId);
            }
        }

        /// <summary>
        /// Creates a new region.
        /// </summary>
        /// <param name="typeId">The block type id</param>
        /// <param name="regionId">The region id</param>
        /// <returns>The new region value</returns>
        public static object CreateRegion(string typeId, string regionId) {
            using (var factory = new ContentFactory(App.BlockTypes)) {
                return factory.CreateRegion(typeId, regionId);
            }
        }
    }

    /// <summary>
    /// Base class for block models.
    /// </summary>
    public abstract class BlockBase : Content
    {
        #region Properties
        /// <summary>
        /// Gets/sets the optional view that should be used
        /// to render this block.
        /// </summary>
        public string View { get; set; }
        #endregion
    }
}
