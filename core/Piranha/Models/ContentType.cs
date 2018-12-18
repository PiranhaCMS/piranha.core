/*
 * Copyright (c) 2016-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Collections.Generic;

namespace Piranha.Models
{
    /// <summary>
    /// Base class for templated content types.
    /// </summary>
    public abstract class ContentType
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets/sets the CLR type of the content model.
        /// </summary>
        public string CLRType { get; set; }

        /// <summary>
        /// Gets/sets the optional title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the content type id.
        /// </summary>
        public string ContentTypeId { get; set; }

        /// <summary>
        /// Gets/sets the available regions.
        /// </summary>
        public IList<RegionType> Regions { get; set; } = new List<RegionType>();

        /// <summary>
        /// Gets/sets the optional routes.
        /// </summary>
        public IList<ContentTypeRoute> Routes { get; set; } = new List<ContentTypeRoute>();
    }
}
