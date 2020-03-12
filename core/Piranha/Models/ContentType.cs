/*
 * Copyright (c) 2020 Piranha CMS
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Piranha.Models
{
    /// <summary>
    /// Class for defining a content type.
    /// </summary>
    [Serializable]
    public sealed class ContentType
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        [Required]
        [StringLength(64)]
        public string Id { get; set; }

        /// <summary>
        /// Gets/sets the full type name of the content model.
        /// </summary>
        [StringLength(255)]
        public string TypeName { get; set; }

        /// <summary>
        /// Gets/sets the full assembly name of the content model.
        /// </summary>
        [StringLength(255)]
        public string AssemblyName { get; set; }

        /// <summary>
        /// Gets/sets the optional title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the content group name.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Gets/sets the available regions.
        /// </summary>
        public IList<RegionType> Regions { get; set; } = new List<RegionType>();

        /// <summary>
        /// Gets/sets the optional routes.
        /// </summary>
        public IList<ContentTypeRoute> Routes { get; set; } = new List<ContentTypeRoute>();

        /// <summary>
        /// Gets/sets the optional custom editors.
        /// </summary>
        public IList<ContentTypeEditor> CustomEditors { get; set; } = new List<ContentTypeEditor>();
    }
}