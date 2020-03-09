/*
 * Copyright (c) 2020 Piranha CMS
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

namespace Piranha.Models
{
    public interface IDynamicContent
    {
        /// <summary>
        /// Gets/sets the regions.
        /// </summary>
        dynamic Regions { get; set; }
    }
}