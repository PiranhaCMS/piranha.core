/*
 * Copyright (c) 2017 Håkan Edling
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
    /// Basic page with main content in markdown.
    /// </summary>
    [PageType(Title = "Teaser Page", Route = "/teaserpage")]
    public class TeaserPage : BasePage<TeaserPage>
    {
        /// <summary>
        /// Gets/sets the available teasers.
        /// </summary>
        [Region(ListTitle = "Title", ListPlaceholder = "New Teaser")]
        public IList<Regions.Teaser> Teasers { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TeaserPage() {
            Teasers = new List<Regions.Teaser>();
        }
    }
}
