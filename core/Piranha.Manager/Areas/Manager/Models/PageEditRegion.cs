/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha.Areas.Manager.Models
{
    /// <summary>
    /// A non collection region.
    /// </summary>
    public class PageEditRegion : PageEditRegionBase
    {
        /// <summary>
        /// Gets/sets the available fields.
        /// </summary>
        public PageEditFieldSet FieldSet { get; set; }

        /// <summary>
        /// Adds a field set to the region.
        /// </summary>
        /// <param name="fieldSet">The field set</param>
        public override void Add(PageEditFieldSet fieldSet) {
            FieldSet = fieldSet;
        }
    }    
}