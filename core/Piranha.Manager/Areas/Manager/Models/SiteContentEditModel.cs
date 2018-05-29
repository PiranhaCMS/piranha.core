/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using System.Collections.Generic;
using Piranha.Manager;

namespace Piranha.Areas.Manager.Models
{
    /// <summary>
    /// The page edit view model.
    /// </summary>
    public class SiteContentEditModel : Piranha.Models.SiteContentBase
    {
        /// <summary>
        /// Gets/sets the site type.
        /// </summary>
        public Piranha.Models.SiteType SiteType { get; set; }

        /// <summary>
        /// Gets/sets the available regions.
        /// </summary>
        public IList<PageEditRegionBase> Regions { get; set; }

         /// <summary>
        /// Default constructor.
        /// </summary>
        public SiteContentEditModel() {
            Regions = new List<PageEditRegionBase>();
        }  
    }
}