/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Models
{
    public interface IDynamicBlockContent
    {
        /// <summary>
        /// Gets/sets the available blocks. This will contain a subset of
        /// the blocks availale in the Sections dictionary.
        /// </summary>
        IList<Extend.Block> Blocks { get; set; }

        /// <summary>
        /// Gets/sets the available block sections.
        /// </summary>
        IDictionary<string, IList<Extend.Block>> Sections { get; set; }
    }
}
