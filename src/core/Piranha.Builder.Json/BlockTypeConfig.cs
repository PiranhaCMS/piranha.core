/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Collections.Generic;

namespace Piranha.Builder.Json
{
    /// <summary>
    /// Config file for importing block types with json.
    /// </summary>
    internal class BlockTypeConfig
    {
        /// <summary>
        /// The available block types.
        /// </summary>
        public IList<Extend.BlockType> BlockTypes { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BlockTypeConfig() {
            BlockTypes = new List<Extend.BlockType>();
        }

        /// <summary>
        /// Asserts that the block types are valid.
        /// </summary>
        public void AssertConfigIsValid() {
            foreach (var blockType in BlockTypes)
                blockType.Ensure();

        }
    }
}
