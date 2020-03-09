/*
 * Copyright (c) 2020 Piranha CMS
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System.Collections.Generic;
using Piranha.Extend;

namespace Piranha.Models
{
    /// <summary>
    /// Base class for all pages.
    /// </summary>
    [ContentGroup(Title = "Page", DefaultRoute = "/page")]
    public abstract class Page : RoutedContent, IBlockContent
    {
        /// <summary>
        /// Gets/sets the available blocks.
        /// </summary>
        public IList<Block> Blocks { get; set; }
    }
}