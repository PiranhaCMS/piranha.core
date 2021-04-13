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
using Piranha.Manager.Models;
using Piranha.Manager.Services;

namespace Piranha.Manager.Controllers
{
    [Area("Manager")]
    [Route("manager/api/labs")]
    [ApiController]
    [Authorize(Policy = Permission.Admin)]
    public class LabsApiController : Controller
    {
        private readonly ContentServiceLabs _service;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="service">The content service</param>
        public LabsApiController(ContentServiceLabs service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets the content model for the page with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The content model</returns>
        [HttpGet]
        [Route("page/{id}")]
        [Authorize(Policy = Permission.PagesEdit)]
        public Task<ContentModel> GetPageAsync(Guid id)
        {
            return _service.GetPageByIdAsync(id);
        }

        /// <summary>
        /// Saves the given page.
        /// </summary>
        /// <param name="model">The content model</param>
        /// <returns>The updated content model</returns>
        [HttpPost]
        [Route("page")]
        [Authorize(Policy = Permission.PagesPublish)]
        public Task<ContentModel> SavePageAsync(ContentModel model)
        {
            return _service.SavePageAsync(model);
        }

        /// <summary>
        /// Saves the given page as a draft.
        /// </summary>
        /// <param name="model">The content model</param>
        /// <returns>The updated content model</returns>
        [HttpPost]
        [Route("page/draft")]
        [Authorize(Policy = Permission.PagesSave)]
        public Task<ContentModel> SavePageDraftAsync(ContentModel model)
        {
            return _service.SavePageAsync(model, true);
        }

        /// <summary>
        /// Reverts the page with the given id to its currently
        /// published version.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The reverted content model</returns>
        [HttpPost]
        [Route("page/revert")]
        [Authorize(Policy = Permission.PagesSave)]
        public Task<ContentModel> RevertPageAsync([FromBody]Guid id)
        {
            return _service.RevertPageAsync(id);
        }

        /// <summary>
        /// Unpublishes the page with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The updated content model</returns>
        [HttpPost]
        [Route("page/unpublish")]
        [Authorize(Policy = Permission.PagesSave)]
        public Task<ContentModel> UnpublishPageAsync([FromBody]Guid id)
        {
            return _service.UnpublishPageAsync(id);
        }

        /// <summary>
        /// Gets the content model for the post with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The content model</returns>
        [HttpGet]
        [Route("post/{id}")]
        [Authorize(Policy = Permission.PostsEdit)]
        public Task<ContentModel> GetPostAsync(Guid id)
        {
            return _service.GetPostByIdAsync(id);
        }

        /// <summary>
        /// Saves the given post.
        /// </summary>
        /// <param name="model">The content model</param>
        /// <returns>The updated content model</returns>
        [HttpPost]
        [Route("post")]
        [Authorize(Policy = Permission.PostsPublish)]
        public Task<ContentModel> SavePostAsync(ContentModel model)
        {
            return _service.SavePostAsync(model);
        }

        /// <summary>
        /// Saves the given post as a draft.
        /// </summary>
        /// <param name="model">The content model</param>
        /// <returns>The updated content model</returns>
        [HttpPost]
        [Route("post/draft")]
        [Authorize(Policy = Permission.PostsSave)]
        public Task<ContentModel> SavePostDraftAsync(ContentModel model)
        {
            return _service.SavePostAsync(model, true);
        }

        /// <summary>
        /// Gets the content model for the content with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="langId">The optional language id</param>
        /// <returns>The content model</returns>
        [HttpGet]
        [Route("content/{id}/{langId?}")]
        [Authorize(Policy = Permission.ContentEdit)]
        public Task<ContentModel> GetContentAsync(Guid id, Guid? langId = null)
        {
            return _service.GetContentByIdAsync(id, langId);
        }

        /// <summary>
        /// Saves the given content.
        /// </summary>
        /// <param name="model">The content model</param>
        /// <returns>The updated content model</returns>
        [HttpPost]
        [Route("content")]
        [Authorize(Policy = Permission.ContentSave)]
        public Task<ContentModel> SaveContentAsync(ContentModel model)
        {
            return _service.SaveContentAsync(model);
        }
    }
}