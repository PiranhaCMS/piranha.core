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
using Piranha.Models;

namespace Piranha.Extend
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BlockGroupTypeAttribute : BlockTypeAttribute
    {
        /// <summary>
        /// Gets/sets how the blocks inside the group should be
        /// displayed in the manager interface.
        /// </summary>
        public BlockDisplayMode Display { get; set; } = BlockDisplayMode.MasterDetail;

        /// <summary>
        /// Gets/sets if the block group should use a custom
        /// view for rendering block global fields. The default
        /// value for this property is false.
        /// </summary>
        public bool UseCustomView { get; set; }
    }
}
