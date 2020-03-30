/*
 * Copyright (c) .NET Foundation and Contributors
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

namespace Piranha.Data
{
    [Serializable]
    public abstract class ContentBase<T> where T : ContentFieldBase
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
	    public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the main title.
        /// </summary>
	    public string Title { get; set; }

        /// <summary>
        /// Gets/sets the created date.
        /// </summary>
	    public DateTime Created { get; set; }

        /// <summary>
        /// Gets/sets the last modification date.
        /// </summary>
	    public DateTime LastModified { get; set; }

        /// <summary>
        /// Gets/sets the available fields.
        /// </summary>
        public IList<T> Fields { get; set; } = new List<T>();
    }
}