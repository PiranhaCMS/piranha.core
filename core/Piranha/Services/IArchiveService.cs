/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Threading.Tasks;
using Piranha.Models;

namespace Piranha.Services
{
    public interface IArchiveService
    {
        [Obsolete("The archive service now only loads the PostArchive and not the page. Please refer to GetByIdAsync<T>()", true)]
        T GetById<T>(Guid id, int? page = 1, Guid? categoryId = null, int? year = null, int? month = null, int? pageSize = null) where T : Models.ArchivePage<T>;

        [Obsolete("The archive service now only loads the PostArchive and not the page. Please refer to GetByIdAsync<T>()", true)]
        T GetById<T>(Guid id, int? page = 1, int? year = null, int? month = null, int? pageSize = null) where T : Models.ArchivePage<T>;

        [Obsolete("The archive service now only loads the PostArchive and not the page. Please refer to GetByIdAsync<T>()", true)]
        T GetByCategoryId<T>(Guid id, Guid categoryId, int? page = 1, int? year = null, int? month = null, int? pageSize = null) where T : Models.ArchivePage<T>;

        [Obsolete("The archive service now only loads the PostArchive and not the page. Please refer to GetByIdAsync<T>()", true)]
        T GetByTagId<T>(Guid id, Guid tagId, int? page = 1, int? year = null, int? month = null, int? pageSize = null) where T : Models.ArchivePage<T>;

        /// <summary>
        /// Gets the post archive for the specified archive page
        /// with the given filters applied.
        /// </summary>
        /// <param name="archiveId">The unique archive page id</param>
        /// <param name="currentPage">The optional page number</param>
        /// <param name="categoryId">The optional category id</param>
        /// <param name="tagId">The optional tag id</param>
        /// <param name="year">The optional year</param>
        /// <param name="month">The optional year</param>
        /// <param name="pageSize">The optional page size. If not provided, this will be read from config</param>
        /// <returns>The post archive</returns>
        Task<PostArchive<DynamicPost>> GetByIdAsync(Guid archiveId, int? currentPage = 1,
            Guid? categoryId = null, Guid? tagId = null, int? year = null, int? month = null, int? pageSize = null);

        /// <summary>
        /// Gets the post archive for the specified archive page
        /// with the given filters applied.
        /// </summary>
        /// <param name="archiveId">The unique archive page id</param>
        /// <param name="currentPage">The optional page number</param>
        /// <param name="categoryId">The optional category id</param>
        /// <param name="tagId">The optional tag id</param>
        /// <param name="year">The optional year</param>
        /// <param name="month">The optional year</param>
        /// <param name="pageSize">The optional page size. If not provided, this will be read from config</param>
        /// <typeparam name="T">The post type</typeparam>
        /// <returns>The post archive</returns>
        Task<PostArchive<T>> GetByIdAsync<T>(Guid archiveId, int? currentPage = 1,
            Guid? categoryId = null, Guid? tagId = null, int? year = null, int? month = null, int? pageSize = null)
            where T : Models.PostBase;
    }
}
