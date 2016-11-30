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
using Piranha.Repositories;

namespace Piranha 
{    
    /// <summary>
    /// Interface for the main data service.
    /// </summary>
    public interface IDataService : IDisposable
    {
        /// <summary>
        /// Gets the archive repository.
        /// </summary>
        IArchiveRepository Archives { get; }

        /// <summary>
        /// Gets the block type repository.
        /// </summary>
        IBlockTypeRepository BlockTypes { get; }

        /// <summary>
        /// Gets the category repository.
        /// </summary>
        ICategoryRepository Categories { get; }

        /// <summary>
        /// Gets the media repository.
        /// </summary>
        IMediaRepository Media { get; }

        /// <summary>
        /// Gets the page repository.
        /// </summary>
        IPageRepository Pages { get; }

        /// <summary>
        /// Gets the page type repository.
        /// </summary>
        IPageTypeRepository PageTypes { get; }

        /// <summary>
        /// Gets the post repository.
        /// </summary>
        IPostRepository Posts { get; }

        /// <summary>
        /// Gets the sitemap repository.
        /// </summary>
        ISitemapRepository Sitemap { get; }
    }
}