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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Piranha.Manager.Models;
using Piranha.Manager.Services;
using Piranha.Models;

namespace Piranha.Manager.Controllers
{
    /// <summary>
    /// Api controller for page management.
    /// </summary>
    [Area("Manager")]
    [Route("manager/api/post")]
    [Authorize(Policy = Permission.Admin)]
    [ApiController]
    public class PostApiController : Controller
    {
        private readonly PostService _service;
        private readonly ManagerLocalizer _localizer;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PostApiController(PostService service, ManagerLocalizer localizer)
        {
            _service = service;
            _localizer = localizer;
        }

        /// <summary>
        /// Gets the list model.
        /// </summary>
        /// <returns>The list model</returns>
        [Route("list/{id}")]
        [HttpGet]
        [Authorize(Policy = Permission.Posts)]
        public async Task<PostListModel> List(Guid id)
        {
            return await _service.GetList(id);
        }

        /// <summary>
        /// Gets the post with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The post edit model</returns>
        [Route("{id:Guid}")]
        [HttpGet]
        [Authorize(Policy = Permission.PostsEdit)]
        public async Task<PostEditModel> Get(Guid id)
        {
            return await _service.GetById(id);
        }
    }
}