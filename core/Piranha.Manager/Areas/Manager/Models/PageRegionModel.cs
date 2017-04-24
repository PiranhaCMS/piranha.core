/*
 * Copyright (c) 2017 Håkan Edling
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
    public class PageRegionModel
    {
        public string PageTypeId { get; set; }
        public string RegionTypeId { get; set; }
        public int RegionIndex { get; set; }
        public int ItemIndex { get; set; }
    }
}