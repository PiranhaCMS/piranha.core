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

namespace Piranha.Repositories
{
    public interface ISitemapRepository
    {
        /// <summary>
        /// Gets the hierachical sitemap structure.
        /// </summary>
        /// <param name="onlyPublished">If only published items should be included</param>
        /// <returns>The sitemap</returns>
        IList<Models.SitemapItem> Get(bool onlyPublished = true);
    }
}
