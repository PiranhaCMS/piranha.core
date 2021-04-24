/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/tidyui/coreweb
 *
 */

using System.Collections.Generic;
using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Models;

namespace RazorWeb.Models
{
    /// <summary>
    /// Page with an additional block section.
    /// </summary>
    [PageType(Title = "Aside Page")]
    [ContentTypeRoute(Title = "Default", Route = "/asidepage")]
    public class AsidePage : Page<AsidePage>
    {
        /// <summary>
        /// Gets/sets the aside blocks.
        /// </summary>
        [Section]
        public IList<Block> Aside { get; set; }
    }
}
