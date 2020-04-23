/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha
{
    /// <summary>
    /// Used to configure the different parts of the integrated middleware
    /// routing component.
    /// </summary>
    public class PiranhaRouteConfig
    {
        /// <summary>
        /// Gets/sets if alias routing should be used.
        /// </summary>
        public bool UseAliasRouting { get; set; } = true;

        /// <summary>
        /// Gets/sets if archive routing should be used.
        /// </summary>
        public bool UseArchiveRouting { get; set; } = true;

        /// <summary>
        /// Gets/sets if page routing should be used.
        /// </summary>
        public bool UsePageRouting { get; set; } = true;

        /// <summary>
        /// Gets/sets if post routing should be used.
        /// </summary>
        public bool UsePostRouting { get; set; } = true;

        /// <summary>
        /// Gets/sets if site routing for multiple sites
        /// should be used.
        /// </summary>
        public bool UseSiteRouting { get; set; } = true;

        /// <summary>
        /// Gets/sets if sitemap routing should be used.
        /// </summary>
        public bool UseSitemapRouting { get; set; } = true;

        /// <summary>
        /// Gets/sets if startpage routing for empty URL's
        /// should be used.
        /// </summary>
        public bool UseStartpageRouting { get; set; } = true;
    }
}