/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/tidyui/coreweb
 * 
 */

using Piranha.AttributeBuilder;
using Piranha.Models;
using Piranha.Extend.Fields;
using System.Collections.Generic;

namespace CoreWeb.Models
{
    /// <summary>
    /// Basic site with some header information.
    /// </summary>
    [SiteType(Title = "Standard site")]
    public class StandardSite : SiteContent<StandardSite>
    {
        [Region]
        public Regions.SiteHeader Header { get; set; }

        [Region(ListExpand=false)]
        public IList<ImageField> Images { get; set; }

        public StandardSite() {
            Images = new List<ImageField>();
        }
    }
}
