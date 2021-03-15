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
using Piranha.Extend;
using Piranha.Models;

namespace RazorWeb.Models
{
    [ContentGroup(Title = "Banners", Icon = "fas fa-star")]
    public abstract class Banner<T> : Content<T>, ICategorizedContent
        where T : Banner<T>
    {
        /// <summary>
        /// Gets/sets the banner category.
        /// </summary>
        public Taxonomy Category { get; set; }
    }
}