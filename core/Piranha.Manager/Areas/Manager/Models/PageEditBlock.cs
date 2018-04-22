/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;

namespace Piranha.Areas.Manager.Models
{
    /// <summary>
    /// Base class for page blocks.
    /// </summary>
    public class PageEditBlock
    {
        /// <summary>
        /// Gets/sets the block id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the CLR type.
        /// </summary>
        public string CLRType { get; set; }

        /// <summary>
        /// Gets/sets if this is a reusable block.
        /// </summary>
        public bool IsReusable { get; set; }

        /// <summary>
        /// Gets/sets the block value.
        /// </summary>
        public Extend.Block Value { get; set; }
    }    
}