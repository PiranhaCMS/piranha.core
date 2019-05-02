/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Collections.Generic;
using Piranha.Extend;

namespace Piranha.Manager.Models.Content
{
    /// <summary>
    /// Content edit model.
    /// </summary>
    public abstract class ContentEditModel
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the content type id.
        /// </summary>
        public string TypeId { get; set; }

        /// <summary>
        /// Gets/sets the mandatory title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the available blocks.
        /// </summary>
        public IList<BlockEditModel> Blocks { get; set; } = new List<BlockEditModel>();

        /// <summary>
        /// Gets/sets the available regions.
        /// </summary>
        public IList<RegionEditModel> Regions { get; set; } = new List<RegionEditModel>();
    }
}