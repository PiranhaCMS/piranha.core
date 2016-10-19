/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Collections.Generic;

namespace Piranha.Builder.Json
{
    /// <summary>
    /// Config file for importing page types with json.
    /// </summary>
    internal class PageTypeConfig
    {
        /// <summary>
        /// The available page types.
        /// </summary>
        public IList<Extend.PageType> PageTypes { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PageTypeConfig() {
            PageTypes = new List<Extend.PageType>();
        }

        /// <summary>
        /// Asserts that the page types are valid.
        /// </summary>
        public void AssertConfigIsValid() {
            foreach (var pageType in PageTypes)
                pageType.Ensure();

        }
    }
}
