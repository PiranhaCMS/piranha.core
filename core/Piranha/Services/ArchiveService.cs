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
using Piranha.Repositories;

namespace Piranha.Services
{
    public class ArchiveService : IArchiveService
    {
        private readonly IArchiveRepository _repo;
        private readonly IParamService _paramService;
        private readonly IPostService _postService;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="repo">The main repository</param>
        /// <param name="paramService">The param service</param>
        /// <param name="postService">The post service</param>
        public ArchiveService(IArchiveRepository repo, IParamService paramService, IPostService postService)
        {
            _repo = repo;
            _paramService = paramService;
            _postService = postService;
        }

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
        public Task<PostArchive<DynamicPost>> GetByIdAsync(Guid archiveId, int? currentPage = 1,
            Guid? categoryId = null, Guid? tagId = null, int? year = null, int? month = null, int? pageSize = null)
        {
            return GetByIdAsync<DynamicPost>(archiveId, currentPage,
                categoryId, tagId, year, month, pageSize);
        }

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
        public async Task<PostArchive<T>> GetByIdAsync<T>(Guid archiveId, int? currentPage = 1,
            Guid? categoryId = null, Guid? tagId = null, int? year = null, int? month = null, int? pageSize = null)
            where T : Models.PostBase
        {
            var model = new PostArchive<T>();

            // Ensure page size
            if (!pageSize.HasValue)
            {
                using (var config = new Config(_paramService))
                {
                    // No page size provided, get from config
                    pageSize = config.ArchivePageSize;

                    if (!pageSize.HasValue || pageSize == 0)
                    {
                        // No config available, default to 5
                        pageSize = 5;
                    }
                }
            }

            // Set basic fields
            model.Year = year;
            model.Month = month;

            // Get paging info
            model.TotalPosts = await _repo.GetPostCount(archiveId, categoryId, tagId, year, month).ConfigureAwait(false);
            model.TotalPages = Math.Max(Convert.ToInt32(Math.Ceiling((double)model.TotalPosts / pageSize.Value)), 1);
            model.CurrentPage = Math.Min(Math.Max(1, currentPage.HasValue ? currentPage.Value : 1), model.TotalPages);

            // Set related info
            if (categoryId.HasValue)
            {
                model.Category = await _postService.GetCategoryByIdAsync(categoryId.Value).ConfigureAwait(false);
            }
            if (tagId.HasValue)
            {
                model.Tag = await _postService.GetTagByIdAsync(tagId.Value).ConfigureAwait(false);
            }

            // Get the id of the current posts
            var posts = await _repo.GetPosts(archiveId, pageSize.Value, model.CurrentPage, categoryId, tagId, year, month).ConfigureAwait(false);

            // Get the posts
            foreach (var postId in posts)
            {
                var post = await _postService.GetByIdAsync<T>(postId).ConfigureAwait(false);

                if (post != null)
                {
                    model.Posts.Add(post);
                }
            }
            return model;
        }
    }
}
