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

namespace Piranha.Models
{
    public interface IBlockContent
    {
        /// <summary>
        /// Gets/sets the blocks.
        /// </summary>
        IList<Extend.Block> Blocks { get; set; }
    }
}
