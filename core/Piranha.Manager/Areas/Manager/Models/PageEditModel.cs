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
    /// The page edit view model.
    /// </summary>
    public class PageEditModel : Piranha.Models.PageBase
    {
        /// <summary>
        /// Gets/sets the page type.
        /// </summary>
        public Piranha.Models.PageType PageType { get; set; }

        /// <summary>
        /// Gets/sets the available blocks.
        /// </summary>
        public new IList<ContentEditBlock> Blocks { get; set; } = new List<ContentEditBlock>();

        /// <summary>
        /// Gets/sets the available regions.
        /// </summary>
        public IList<PageEditRegionBase> Regions { get; set; } = new List<PageEditRegionBase>();

        /// <summary>
        /// Gets/sets the page content type.
        /// </summary>
        public Runtime.AppContentType PageContentType { get; set; }
    }
}
