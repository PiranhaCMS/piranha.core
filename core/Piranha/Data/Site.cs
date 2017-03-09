/*
 * Copyright (c) 2017 Håkan Edling
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
    public sealed class Site : IModel, ICreated, IModified
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets/sets the internal textual id.
        /// </summary>
        public string InternalId { get; set; }

        /// <summary>
        /// Gets/sets the main title.
        /// </summary>
        public string Title { get; set; }

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
        /// Gets/sets the created date.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets/sets the last modification date.
        /// </summary>
        public DateTime LastModified { get; set; }
    }
}
