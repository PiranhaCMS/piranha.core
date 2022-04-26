/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Manager.Models.Content
{
    /// <summary>
    /// Meta information for blocks.
    /// </summary>
    public class BlockSection
    {
        /// <summary>
        /// Gets/sets the section id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets/sets the section title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the available blocks.
        /// </summary>
        public IList<BlockModel> Blocks { get; set; } = new List<BlockModel>();
    }
}