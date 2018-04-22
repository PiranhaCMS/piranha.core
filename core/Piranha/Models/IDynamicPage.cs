/*
 * Copyright (c) 2018 Håkan Edling
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
    public interface IDynamicPage
    {
        /// <summary>
        /// Gets/sets the available blocks.
        /// </summary>
        IList<Extend.Block> Blocks { get; set; }        
    }
}
