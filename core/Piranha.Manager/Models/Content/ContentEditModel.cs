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

namespace Piranha.Manager.Models.Content
{
    /// <summary>
    /// Content edit model.
    /// </summary>
    public abstract class ContentEditModel : AsyncResult
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
        /// Gets/sets if blocks should be used.
        /// </summary>
        public bool UseBlocks { get; set; } = true;

        /// <summary>
        /// Gets/sets the available blocks.
        /// </summary>
        public IList<BlockModel> Blocks { get; set; } = new List<BlockModel>();

        /// <summary>
        /// Gets/sets the available regions.
        /// </summary>
        public IList<RegionModel> Regions { get; set; } = new List<RegionModel>();

        /// <summary>
        /// Gets/sets the available custom editors.
        /// </summary>
        public IList<EditorModel> Editors { get; set; } = new List<EditorModel>();
    }
}