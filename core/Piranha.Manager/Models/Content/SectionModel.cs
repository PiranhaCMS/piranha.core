/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Collections.Generic;

namespace Piranha.Manager.Models.Content
{
    /// <summary>
    /// Edit model for sections.
    /// </summary>
    public class SectionModel
    {
        /// <summary>
        /// Gets/sets the section id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets/sets the uid used by the interface.
        /// </summary>
        public string Uid => $"uid-blocks-{ Id }";

        /// <summary>
        /// Gets/sets the section name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets/sets the available blocks.
        /// </summary>
        public IList<BlockModel> Blocks { get; set; } = new List<BlockModel>();
    }
}