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
        /// <param name="pageSize">The optional page size</param>
        /// <returns>The archive model</returns>
        [Obsolete("Please update your code to use the new GetById, GetByCategoryId & GetByTagId", true)]
        T GetById<T>(Guid id, int? page = 1, Guid? categoryId = null, int? year = null, int? month = null, int? pageSize = null) where T : BlogPage<T>;

        /// <summary>
        /// Gets the post archive for the blog with the given id.
        /// </summary>
        /// <param name="id">The unique blog id</param>
        /// <param name="page">The optional page</param>
        /// <param name="year">The optional year</param>
        /// <param name="month">The optional month</param>
        /// <param name="pageSize">The optional page size</param>
        /// <returns>The archive model</returns>
        T GetById<T>(Guid id, int? page = 1, int? year = null, int? month = null, int? pageSize = null) where T : BlogPage<T>;

        /// <summary>
        /// Gets the post archive for the blog with the given id.
        /// </summary>
        /// <param name="id">The unique blog id</param>
        /// <param name="categoryId">The unique category id</param>
        /// <param name="page">The optional page</param>
        /// <param name="year">The optional year</param>
        /// <param name="month">The optional month</param>
        /// <param name="pageSize">The optional page size</param>
        /// <returns>The archive model</returns>
        T GetByCategoryId<T>(Guid id, Guid categoryId, int? page = 1, int? year = null, int? month = null, int? pageSize = null) where T : BlogPage<T>;

        /// <summary>
        /// Gets the post archive for the blog with the given id.
        /// </summary>
        /// <param name="id">The unique blog id</param>
        /// <param name="tagId">The unique tag id</param>
        /// <param name="page">The optional page</param>
        /// <param name="year">The optional year</param>
        /// <param name="month">The optional month</param>
        /// <param name="pageSize">The optional page size</param>
        /// <returns>The archive model</returns>
        T GetByTagId<T>(Guid id, Guid tagId, int? page = 1, int? year = null, int? month = null, int? pageSize = null) where T : BlogPage<T>;
    }
}
