/*
 * Copyright (c) 2017-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Piranha.Data;
using Piranha.Repositories;
using System;

namespace Piranha
{
    /// <summary>
    /// The main application api.
    /// </summary>
    public interface IApi : IDisposable
    {
        /// <summary>
        /// Gets/sets the alias repository.
        /// </summary>
        IAliasRepository Aliases { get; }

        /// <summary>
        /// Gets/sets the archive repository.
        /// </summary>
        IArchiveRepository Archives { get; }

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
        /// Gets the param repository.
        /// </summary>
        IParamRepository Params { get; }

        /// <summary>
        /// Gets the post repository.
        /// </summary>
        IPostRepository Posts { get; }

        /// <summary>
        /// Gets the post type repository.
        /// </summary>
        IPostTypeRepository PostTypes { get; }

        /// <summary>
        /// Gets the site repository.
        /// </summary>
        ISiteRepository Sites { get; }

        /// <summary>
        /// Gets the site type repository.
        /// </summary>
        ISiteTypeRepository SiteTypes { get; }

        /// <summary>
        /// Gets the tag repository.
        /// </summary>
        ITagRepository Tags { get; }
    }
}
