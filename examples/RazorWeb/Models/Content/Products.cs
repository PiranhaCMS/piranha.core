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
    [ContentGroup(Title = "Products")]
    public abstract class Product<T> : Content<T>, ICategorizedContent
        where T : Product<T>
    {
        /// <summary>
        /// Gets/sets the product category.
        /// </summary>
        public Taxonomy Category { get; set; }
    }

    /// <summary>
    /// Basic product.
    /// </summary>
    [ContentType(Title = "Standard Product")]
    public class StandardProduct : Product<StandardProduct>
    {
        [Region(Title = "All fields")]
        public Regions.AllFields AllFields { get; set; }
    }

    [ContentType(Title = "Special Product")]
    public class SpecialProduct : Product<SpecialProduct>
    {
    }
}
