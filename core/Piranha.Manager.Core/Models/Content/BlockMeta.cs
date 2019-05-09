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

namespace Piranha.Manager.Models.Content
{
    /// <summary>
    /// Meta information for blocks.
    /// </summary>
    public class BlockMeta : ContentMeta
    {
        /// <summary>
        /// Gets/sets if this is a block group.
        /// </summary>
        public bool IsGroup { get; set; } = false;
    }
}