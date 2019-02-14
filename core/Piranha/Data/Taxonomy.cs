/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.ComponentModel.DataAnnotations;

namespace Piranha.Data
{
    [Serializable]
    public abstract class Taxonomy
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the title.
        /// </summary>
        [Required]
        [StringLength(128)]
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the slug.
        /// </summary>
        [StringLength(128)]
        public string Slug { get; set; }

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
