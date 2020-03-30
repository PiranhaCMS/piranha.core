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

namespace Piranha.Data
{
    /// <summary>
    /// Reusable content block.
    /// </summary>
    [Serializable]
    public sealed class Block
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// This is not part of the data model. It's only used
        /// for internal mapping.
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Gets/sets the CLR type of the block.
        /// </summary>
        public string CLRType { get; set; }

        /// <summary>
        /// Gets/sets the optional title. This property
        /// is only used for reusable blocks within the
        /// block library.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets if this is a reusable block.
        /// </summary>
        public bool IsReusable { get; set; }

        /// <summary>
        /// Gets/sets the created date.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets/sets the last modification date.
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Gets/sets the available fields.
        /// </summary>
        public IList<BlockField> Fields { get; set; } = new List<BlockField>();
    }
}
