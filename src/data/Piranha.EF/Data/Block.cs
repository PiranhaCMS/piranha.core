/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using System.Collections.Generic;

namespace Piranha.EF.Data
{
    public sealed class Block : IModel, ICreated, IModified
    {
        #region Properties
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the block type id.
        /// </summary>
        public string TypeId { get; set; }

        /// <summary>
        /// Gets/sets the main title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the optional view that should be used
        /// to render this block.
        /// </summary>
        public string View { get; set; }

        /// <summary>
        /// Gets/sets the optional published date.
        /// </summary>
        public DateTime? Published { get; set; }

        /// <summary>
        /// Gets/sets the created date.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets/sets the last modification date.
        /// </summary>
        public DateTime LastModified { get; set; }
        #endregion

        #region Navigation properties
        /// <summary>
        /// Gets/sets the block type.
        /// </summary>
        public BlockType Type { get; set; }

        /// <summary>
        /// Gets/sets the available fields.
        /// </summary>
        public IList<BlockField> Fields { get; set; }
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Block() {
            Fields = new List<BlockField>();
        }
    }
}
