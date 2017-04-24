/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Collections.Generic;

namespace Piranha.Areas.Manager.Models
{
    /// <summary>
    /// A collection region.
    /// </summary>
    public class PageEditRegionCollection : PageEditRegionBase
    {
        /// <summary>
        /// Gets/sets the available fieldsets.
        /// </summary>
        public IList<PageEditFieldSet> FieldSets { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PageEditRegionCollection() {
            FieldSets = new List<PageEditFieldSet>();
        }

        /// <summary>
        /// Adds a field set to the region.
        /// </summary>
        /// <param name="fieldSet">The field set</param>
        public override void Add(PageEditFieldSet fieldSet) {
            FieldSets.Add(fieldSet);
        }
    }
}