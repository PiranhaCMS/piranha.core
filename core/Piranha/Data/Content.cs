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
using System.Collections.Generic;

namespace Piranha.Data
{
    public abstract class Content<T> where T : ContentField
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