/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Piranha.Models
{
    /// <summary>
    /// Abstract base class for templated content.
    /// </summary>
    [Serializable]
    public abstract class ContentBase
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the content type id.
        /// </summary>
        //[Required]
        [StringLength(64)]
        public string TypeId { get; set; }

        /// <summary>
        /// Gets/sets the title.
        /// </summary>
        //[Required]
        [StringLength(128)]
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the permissions needed to access the page.
        /// </summary>
        public IList<string> Permissions { get; set; } = new List<string>();

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
