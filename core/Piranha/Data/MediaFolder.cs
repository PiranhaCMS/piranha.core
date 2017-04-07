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
    public sealed class MediaFolder : IModel, ICreated 
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets/sets the optional parent id.
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// Gets/sets the folder name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets/sets the created date.
        /// </summary>
        public DateTime Created { get; set; }
    }
}