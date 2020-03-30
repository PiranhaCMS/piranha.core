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

namespace Piranha.Data
{
    /// <summary>
    /// Connection between a page and a content.
    /// </summary>
    public interface IContentBlock
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the block id.
        /// </summary>
        Guid BlockId { get; set; }

        /// <summary>
        /// Gets/sets the zero based sort index.
        /// </summary>
        int SortOrder { get; set; }

        /// <summary>
        /// Gets/sets the block data.
        /// </summary>
        Block Block { get; set; }
    }
}
