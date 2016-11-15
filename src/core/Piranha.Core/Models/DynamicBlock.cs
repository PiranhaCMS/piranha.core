/*
 * Copyright (c) 2016 Håkan Edling
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
    /// Dynamic block model.
    /// </summary>
    public class DynamicBlock : Block<DynamicBlock>
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
        public DynamicBlock() : base() {
            Regions = new ExpandoObject();
        }
    }
}