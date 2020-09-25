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

namespace Piranha.Models
{
    /// <summary>
    /// Interface for generic content models.
    /// </summary>
    public interface IContent
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the content type id.
        /// </summary>
        string TypeId { get; set; }

        /// <summary>
        /// Gets/sets the title.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets/sets the permissions needed to access the page.
        /// </summary>
        IList<string> Permissions { get; set; }

        /// <summary>
        /// Gets/sets the created date.
        /// </summary>
        DateTime Created { get; set; }

        /// <summary>
        /// Gets/sets the last modification date.
        /// </summary>
        DateTime LastModified { get; set; }
    }
}