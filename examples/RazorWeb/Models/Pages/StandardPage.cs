/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/tidyui/coreweb
 *
 */

using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Models;

namespace RazorWeb.Models
{
    /// <summary>
    /// Basic page with main content in markdown.
    /// </summary>
    [PageType(Title = "Standard Page")]
    [ContentTypeRoute(Title = "Narrow", Route = "/page")]
    [ContentTypeRoute(Title = "Wide", Route = "/pagewide")]
    public class StandardPage : Page<StandardPage>
    {
        /// <summary>
        /// Gets/sets the page header.
        /// </summary>
        [Region]
        public Regions.Hero Hero { get; set; }
    }
}
