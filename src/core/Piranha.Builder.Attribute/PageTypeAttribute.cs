/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;

namespace Piranha.Builder.Attribute
{
    /// <summary>
    /// Attribute for marking a class as a page type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PageTypeAttribute : System.Attribute
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets/sets the optional title. 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the optional route.
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// Gets/sets the optional view.
        /// </summary>
        public string View { get; set; }
    }
}
