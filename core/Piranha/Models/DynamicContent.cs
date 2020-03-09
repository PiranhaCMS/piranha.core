/*
 * Copyright (c) 2020 Piranha CMS
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System.Collections.Generic;
using System.Dynamic;
using Piranha.Extend;

namespace Piranha.Models
{
    /// <summary>
    /// Dynamic content model.
    /// </summary>
    public sealed class DynamicContent : Content, IDynamicContent, IBlockContent
    {
        /// <summary>
        /// Gets/sets the regions.
        /// </summary>
        public dynamic Regions { get; set; } = new ExpandoObject();

        /// <summary>
        /// Gets/sets the available blocks.
        /// </summary>
        public IList<Block> Blocks { get; set; }
    }
}