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
using Piranha.Models;

namespace Piranha.Repositories
{
    public interface IArchiveRepository
    {
        /// <summary>
        /// Gets the post archive for the blog with the given id.
        /// </summary>
        /// <param name="id">The unique blog id</param>
        /// <param name="page">The optional page</param>
        /// <param name="categoryId">The optional category id</param>
        /// <param name="year">The optional year</param>
        /// <param name="month">The optional month</param>
        /// <returns>The archive model</returns>
        T GetById<T>(Guid id, int? page = 1, Guid? categoryId = null, int? year = null, int? month = null) where T : BlogPage<T>;

        /// <summary>
        /// Gets the post archive for the blog with the given slug.
        /// </summary>
        /// <param name="slug">The unique blog slug</param>
        /// <param name="page">The optional page</param>
        /// <param name="categoryId">The optional category id</param>
        /// <param name="year">The optional year</param>
        /// <param name="month">The optional month</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The archive model</returns>
        T GetBySlug<T>(string slug, int? page = 1, Guid? categoryId = null, int? year = null, int? month = null, Guid? siteId = null) where T : BlogPage<T>;
    }
}
