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
using System.Linq;
using System.Threading.Tasks;
using Piranha.Repositories;

namespace Piranha.Services
{
    public class ArchiveService
    {
        private readonly IArchiveRepository _repo;
        private readonly PageService _pageService;
        private readonly ParamService _paramService;
        private readonly PostService _postService;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="repo">The main repository</param>
        /// <param name="pageService">The page service</param>
        /// <param name="paramService">The param service</param>
        /// <param name="postService">The post service</param>
        public ArchiveService(IArchiveRepository repo, PageService pageService, ParamService paramService, PostService postService)
        {
            _repo = repo;
            _pageService = pageService;
            _paramService = paramService;
            _postService = postService;
        }

        [Obsolete("Please refer to GetById<T>(archiveId, page, categoryId, tagId, year, month, pageSize)", true)]
        public T GetById<T>(Guid id, int? page = 1, Guid? categoryId = null, int? year = null, int? month = null, int? pageSize = null) where T : Models.ArchivePage<T>
        {
            return null;
        }

        [Obsolete("Please refer to GetById<T>(archiveId, page, categoryId, tagId, year, month, pageSize)", true)]
        public T GetById<T>(Guid id, int? page = 1, int? year = null, int? month = null, int? pageSize = null) where T : Models.ArchivePage<T>
        {
            return null;
        }

        [Obsolete("Please refer to GetById<T>(archiveId, page, categoryId, tagId, year, month, pageSize)", true)]
        public T GetByCategoryId<T>(Guid id, Guid categoryId, int? page = 1, int? year = null, int? month = null, int? pageSize = null) where T : Models.ArchivePage<T>
        {
            return null;
        }

        [Obsolete("Please refer to GetById<T>(archiveId, page, categoryId, tagId, year, month, pageSize)", true)]
        public T GetByTagId<T>(Guid id, Guid tagId, int? page = 1, int? year = null, int? month = null, int? pageSize = null) where T : Models.ArchivePage<T>
        {
            return null;
        }

        /// <summary>
        /// Gets the specified post archive for the specified filter.
        /// </summary>
        /// <param name="archiveId">The archive page id</param>
        /// <param name="currentPage">The current page of the archive</param>
        /// <param name="categoryId">The optional category id</param>
        /// <param name="tagId">The optional tag id</param>
        /// <param name="year">The optional year</param>
        /// <param name="month">The optional month</param>
        /// <param name="pageSize">The optional page size</param>
        /// <typeparam name="T">The archive model type</typeparam>
        /// <returns>The archive model</returns>
        public async Task<T> GetByIdAsync<T>(Guid archiveId, int? currentPage = 1, Guid? categoryId = null, Guid? tagId = null, int? year = null, int? month = null, int? pageSize = null) where T : Models.ArchivePage<T>
        {
            // Get the requested blog page
            var model = await _pageService
                .GetByIdAsync<T>(archiveId)
                .ConfigureAwait(false);

            if (model != null)
            {
                // Ensure page size
                if (!pageSize.HasValue)
                {
                    using (var config = new Config(_paramService))
                    {
                        pageSize = config.ArchivePageSize;

                        if (!pageSize.HasValue || pageSize == 0)
                        {
                            pageSize = 5;
                        }
                    }
                }

                // Set basic fields
                model.Archive = new Models.PostArchive();
                model.Route = model.Route ?? "/archive";
                model.Archive.Year = year;
                model.Archive.Month = month;

                // Get paging info
                model.Archive.TotalPosts = await _repo.GetPostCount(archiveId, categoryId, tagId, year, month).ConfigureAwait(false);
                model.Archive.TotalPages = Math.Max(Convert.ToInt32(Math.Ceiling((double)model.Archive.TotalPosts / pageSize.Value)), 1);
                model.Archive.CurrentPage = Math.Min(Math.Max(1, currentPage.HasValue ? currentPage.Value : 1), model.Archive.TotalPages);

                // Get the id of the current posts
                var posts = await _repo.GetPosts(archiveId, pageSize.Value, model.Archive.CurrentPage, categoryId, tagId, year, month).ConfigureAwait(false);

                // Get the posts
                foreach (var postId in posts)
                {
                    var post = await _postService.GetByIdAsync(postId).ConfigureAwait(false);

                    if (post != null)
                    {
                        model.Archive.Posts.Add(post);
                    }
                }
                return model;
            }
            return null;
        }
    }
}
