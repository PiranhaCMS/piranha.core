/*
 * Copyright (c) 2017-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System;

namespace Piranha.Data
{
    public sealed class Site : Content<SiteField>, IModel, ICreated, IModified
    {
        /// <summary>
        /// Gets/sets the optional site type id.
        /// </summary>
        public string SiteTypeId { get; set; }

        /// <summary>
        /// Gets/sets the internal textual id.
        /// </summary>
        public string InternalId { get; set; }

        /// <summary>
        /// Gets/sets the optional description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets/sets the optional hostnames to bind this site for.
        /// </summary>
        public string Hostnames { get; set; }

        /// <summary>
        /// Gets/sets if this is the default site.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets/sets the optional culture for the site.
        /// </summary>
        public string Culture { get; set; }

        /// <summary>
        /// Gets/sets the global last modification date
        /// of the site's content.
        /// </summary>
        public DateTime? ContentLastModified { get; set; }
    }
}
