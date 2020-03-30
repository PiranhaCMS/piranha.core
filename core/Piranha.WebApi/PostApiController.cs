/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Models;

namespace Piranha.WebApi
{
    [ApiController]
    [Route("api/post")]
    [Authorize(Policy = Permissions.Posts)]
    public class PostApiController : Controller
    {
        private readonly IApi _api;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public PostApiController(IApi api)
        {
            _api = api;
        }

        /// <summary>
        /// Gets the full post model for the post with
        /// the specified id.
        /// </summary>
        /// <param name="id">The post id</param>
        /// <returns>The post model</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        public Task<PostBase> GetById(Guid id)
        {
            return _api.Posts.GetByIdAsync<PostBase>(id);
        }

        /// <summary>
        /// Gets the full post model for the post with
        /// the specified archive and slug.
        /// </summary>
        /// <param name="archiveId">The archive id</param>
        /// <param name="slug">The slug</param>
        /// <returns>The post model</returns>
        [HttpGet]
        [Route("{archiveId}/{slug}")]
        public Task<PostBase> GetBySlugAndArchive(Guid archiveId, string slug)
        {
            return _api.Posts.GetBySlugAsync<PostBase>(archiveId, slug);
        }

        /// <summary>
        /// Gets the post info model for the post with
        /// the specified id.
        /// </summary>
        /// <param name="id">The post id</param>
        /// <returns>The post model</returns>
        [HttpGet]
        [Route("info/{id:Guid}")]
        public Task<PostInfo> GetInfoById(Guid id)
        {
            return _api.Posts.GetByIdAsync<PostInfo>(id);
        }

        /// <summary>
        /// Gets the post info model for the post with
        /// the specified archive and slug.
        /// </summary>
        /// <param name="archiveId">The archive id</param>
        /// <param name="slug">The slug</param>
        /// <returns>The post model</returns>
        [HttpGet]
        [Route("info/{archiveId}/{slug}")]
        public Task<PostInfo> GetInfoBySlugAndSite(Guid archiveId, string slug)
        {
            return _api.Posts.GetBySlugAsync<PostInfo>(archiveId, slug);
        }
    }
}