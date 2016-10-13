/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha.Models
{
    public class CategoryModel : Category
    {
        #region Properties
        /// <summary>
        /// Gets/sets the archive title.
        /// </summary>
        public string ArchiveTitle { get; set; }

        /// <summary>
        /// Gets/sets the archive meta keywords.
        /// </summary>
        public string ArchiveKeywords { get; set; }

        /// <summary>
        /// Gets/sets the archive meta description.
        /// </summary>
        public string ArchiveDescription { get; set; }

        /// <summary>
        /// Gets/sets the archive route.
        /// </summary>
        public string ArchiveRoute { get; set; }
        #endregion
    }
}
