/*
 * Copyright (c) 2017-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using Piranha.Repositories;
using Piranha.Services;

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
        AliasService Aliases { get; }

        /// <summary>
        /// Gets/sets the archive service.
        /// </summary>
        ArchiveService Archives { get; }

        /// <summary>
        /// Gets the media service.
        /// </summary>
        MediaService Media { get; }

        /// <summary>
        /// Gets the page repository.
        /// </summary>
        PageService Pages { get; }

        /// <summary>
        /// Gets the page type service.
        /// </summary>
        PageTypeService PageTypes { get; }

        /// <summary>
        /// Gets the param service.
        /// </summary>
        ParamService Params { get; }

        /// <summary>
        /// Gets the post service.
        /// </summary>
        PostService Posts { get; }

        /// <summary>
        /// Gets the post type service.
        /// </summary>
        PostTypeService PostTypes { get; }

        /// <summary>
        /// Gets the site service.
        /// </summary>
        SiteService Sites { get; }

        /// <summary>
        /// Gets the site type service.
        /// </summary>
        SiteTypeService SiteTypes { get; }
    }
}
