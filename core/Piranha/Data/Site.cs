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
using System.ComponentModel.DataAnnotations;

namespace Piranha.Data
{
    [Serializable]
    public sealed class Site : Content<SiteField>, IModel, ICreated, IModified
    {
        /// <summary>
        /// Gets/sets the optional site type id.
        /// </summary>
        [StringLength(64)]
        public string SiteTypeId { get; set; }

        /// <summary>
        /// Gets/sets the internal textual id.
        /// </summary>
        [StringLength(64)]
        public string InternalId { get; set; }

        /// <summary>
        /// Gets/sets the optional description.
        /// </summary>
        [StringLength(256)]
        public string Description { get; set; }

        /// <summary>
        /// Gets/sets the optional hostnames to bind this site for.
        /// </summary>
        [StringLength(256)]
        public string Hostnames { get; set; }

        /// <summary>
        /// Gets/sets if this is the default site.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets/sets the optional culture for the site.
        /// </summary>
        [StringLength(6)]
        public string Culture { get; set; }

        /// <summary>
        /// Gets/sets the global last modification date
        /// of the site's content.
        /// </summary>
        public DateTime? ContentLastModified { get; set; }
    }
}
