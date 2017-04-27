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
    /// A collection of page fields.
    /// </summary>
    public class PageEditFieldSet : List<PageEditField> 
    { 
        /// <summary>
        /// Gets/sets the possible list title.
        /// </summary>
        public string ListTitle { get; set; }

        /// <summary>
        /// Gets/sets if the field set shouldn't be
        /// expandable in lists.
        /// </summary>
        public bool NoExpand { get; set; }
    }    
}