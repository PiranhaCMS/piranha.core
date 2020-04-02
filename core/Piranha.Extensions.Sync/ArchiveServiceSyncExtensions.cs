/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using Piranha.Models;

namespace Piranha.Services
{
    public static class ArchiveServiceSyncExtensions
    {
        /// <summary>
        /// Gets the specified post archive for the specified filter.
        /// </summary>
        /// <param name="service">The archive service</param>
        /// <param name="archiveId">The archive page id</param>
        /// <param name="currentPage">The current page of the archive</param>
        /// <param name="categoryId">The optional category id</param>
        /// <param name="tagId">The optional tag id</param>
        /// <param name="year">The optional year</param>
        /// <param name="month">The optional month</param>
        /// <param name="pageSize">The optional page size</param>
        /// <typeparam name="T">The post model type</typeparam>
        /// <returns>The archive model</returns>
        [Obsolete]
        public static PostArchive<T> GetById<T>(this IArchiveService service, Guid archiveId, int? currentPage = 1, Guid? categoryId = null,
            Guid? tagId = null, int? year = null, int? month = null, int? pageSize = null) where T : PostBase
        {
            return service.GetByIdAsync<T>(archiveId, currentPage, categoryId, tagId, year, month, pageSize)
                .GetAwaiter()
                .GetResult();
        }
    }
}
