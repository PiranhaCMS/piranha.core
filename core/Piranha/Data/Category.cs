/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;

namespace Piranha.Data
{
    public sealed class Category : IModel, ICreated, IModified
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the slug.
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Gets/sets the optional description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets/sets if an archive should be enabled
        /// for the category.
        /// </summary>
        public bool EnableArchive { get; set; }

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

        /// <summary>
        /// Gets/sets the created date.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets/sets the last modification date.
        /// </summary>
        public DateTime LastModified { get; set; }
    }
}
