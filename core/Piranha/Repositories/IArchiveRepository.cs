/*
 * Copyright (c) 2016-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Piranha.Repositories
{
    public interface IArchiveRepository
    {
        /// <summary>
        /// Gets the total post count for the specified archive
        /// and filter.
        /// </summary>
        /// <param name="archiveId">The archive id</param>
        /// <param name="categoryId">The optional category id</param>
        /// <param name="tagId">The optional tag id</param>
        /// <param name="year">The optional year</param>
        /// <param name="month">The optional month</param>
        /// <returns>The total post count</returns>
        Task<int> GetPostCount(Guid archiveId, Guid? categoryId = null, Guid? tagId = null, int? year = null, int? month = null);

        /// <summary>
        /// Gets the id of the posts in the current archive
        /// matching the specified filter.
        /// </summary>
        /// <param name="archiveId">The archive id</param>
        /// <param name="pageSize">The page size</param>
        /// <param name="currentPage">The current page</param>
        /// <param name="categoryId">The optional category id</param>
        /// <param name="tagId">The optional tag id</param>
        /// <param name="year">The optional year</param>
        /// <param name="month">The optional month</param>
        /// <returns>The matching posts</returns>
        Task<IEnumerable<Guid>> GetPosts(Guid archiveId, int pageSize, int currentPage, Guid? categoryId = null, Guid? tagId = null, int? year = null, int? month = null);
    }
}
