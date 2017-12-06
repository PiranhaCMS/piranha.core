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

namespace Piranha.Data
{
    public sealed class Tag : Taxonomy, IModel, ICreated, IModified 
    { 
        /// <summary>
        /// Gets/sets the id of the blog page this
        /// category belongs to.
        /// </summary>
        public Guid BlogId { get; set; }

        /// <summary>
        /// Gets/sets the blog page this category belongs to.
        /// </summary>
        public Page Blog { get; set; }        
    }
}
