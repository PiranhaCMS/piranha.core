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
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Manager.Models;
using Piranha.Manager.Services;

namespace Piranha.Manager.Controllers
{
    [Area("Manager")]
    [Route("manager/api/labs")]
    [ApiController]
    public class LabsApiController : Controller
    {
        private readonly ContentServiceLabs _service;

        public LabsApiController(ContentServiceLabs service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("page/{id}")]
        public async Task<Piranha.Models.PageBase> GetPageAsync(Guid id)
        {
            var model = await _service.GetPageByIdAsync(id);
            return await _service.ToPage(model);
        }

        [HttpGet]
        [Route("post/{id}")]
        public Task<ContentModel> GetPostAsync(Guid id)
        {
            return _service.GetPostByIdAsync(id);
        }

        [HttpGet]
        [Route("content/{id}")]
        public Task<ContentModel> GetContentAsync(Guid id)
        {
            return _service.GetContentByIdAsync(id);
        }
    }
}