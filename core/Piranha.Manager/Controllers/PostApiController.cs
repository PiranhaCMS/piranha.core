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
using Microsoft.AspNetCore.SignalR;
using Piranha.Manager.Models;
using Piranha.Manager.Services;

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
        private readonly IHubContext<Hubs.PreviewHub> _hub;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PostApiController(PostService service, ManagerLocalizer localizer, IHubContext<Hubs.PreviewHub> hub)
        {
            _service = service;
            _localizer = localizer;
            _hub = hub;
        }

        /// <summary>
        /// Gets the list model.
        /// </summary>
        /// <returns>The list model</returns>
        [Route("list/{id}/{index?}")]
        [HttpGet]
        [Authorize(Policy = Permission.Posts)]
        public async Task<PostListModel> List(Guid id, int index = 0)
        {
            return await _service.GetList(id, index);
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

        /// <summary>
        /// Creates a new post of the specified type.
        /// </summary>
        /// <param name="archiveId">The archive id</param>
        /// <param name="typeId">The type id</param>
        /// <returns>The page edit model</returns>
        [Route("create/{archiveId}/{typeId}")]
        [HttpGet]
        [Authorize(Policy = Permission.PostsAdd)]
        public async Task<PostEditModel> Create(Guid archiveId, string typeId)
        {
            return await _service.Create(archiveId, typeId);
        }

        [Route("modal")]
        [HttpGet]
        [Authorize(Policy = Permission.Posts)]
        public async Task<PostModalModel> Modal(Guid? siteId, Guid? archiveId)
        {
            return await _service.GetArchiveMap(siteId, archiveId);
        }

        /// <summary>
        /// Saves the given model
        /// </summary>
        /// <param name="model">The model</param>
        /// <returns>The result of the operation</returns>
        [Route("save")]
        [HttpPost]
        [Authorize(Policy = Permission.PostsPublish)]
        public async Task<PostEditModel> Save(PostEditModel model)
        {
            // Ensure that we have a published date
            if (string.IsNullOrEmpty(model.Published))
            {
                model.Published = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            }

            var ret = await Save(model, false);
            await _hub?.Clients.All.SendAsync("Update", model.Id);

            return ret;
        }

        /// <summary>
        /// Saves the given model
        /// </summary>
        /// <param name="model">The model</param>
        /// <returns>The result of the operation</returns>
        [Route("save/draft")]
        [HttpPost]
        [Authorize(Policy = Permission.PostsSave)]
        public async Task<PostEditModel> SaveDraft(PostEditModel model)
        {
            var ret = await Save(model, true);
            await _hub?.Clients.All.SendAsync("Update", model.Id);

            return ret;
        }

        /// <summary>
        /// Saves the given model and unpublishes it
        /// </summary>
        /// <param name="model">The model</param>
        /// <returns>The result of the operation</returns>
        [Route("save/unpublish")]
        [HttpPost]
        [Authorize(Policy = Permission.PostsPublish)]
        public async Task<PostEditModel> SaveUnpublish(PostEditModel model)
        {
            // Remove published date
            model.Published = null;

            var ret = await Save(model, false);
            await _hub?.Clients.All.SendAsync("Update", model.Id);

            return ret;
        }

        [Route("revert/{id}")]
        [HttpGet]
        [Authorize(Policy = Permission.PostsSave)]
        public async Task<PostEditModel> Revert(Guid id)
        {
            var post = await _service.GetById(id, false);

            if (post != null)
            {
                await _service.Save(post, false);

                post = await _service.GetById(id);
            }

            post.Status = new StatusMessage
            {
                Type = StatusMessage.Success,
                Body = _localizer.Post["The post was successfully reverted to its previous state"]
            };

            await _hub?.Clients.All.SendAsync("Update", id);

            return post;
        }


        /// <summary>
        /// Deletes the post with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The result of the operation</returns>
        [Route("delete/{id}")]
        [HttpGet]
        [Authorize(Policy = Permission.PostsDelete)]
        public async Task<StatusMessage> Delete(Guid id)
        {
            try
            {
                await _service.Delete(id);
            }
            catch (ValidationException e)
            {
                // Validation did not succeed
                return new StatusMessage
                {
                    Type = StatusMessage.Error,
                    Body = e.Message
                };
            }
            catch
            {
                return new StatusMessage
                {
                    Type = StatusMessage.Error,
                    Body = _localizer.Post["An error occured while deleting the post"]
                };
            }

            return new StatusMessage
            {
                Type = StatusMessage.Success,
                Body = _localizer.Page["The post was successfully deleted"]
            };
        }

        /// <summary>
        /// Saves the given model
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="draft">If the page should be saved as a draft</param>
        /// <returns>The result of the operation</returns>
        private async Task<PostEditModel> Save(PostEditModel model, bool draft = false)
        {
            try
            {
                await _service.Save(model, draft);
            }
            catch (ValidationException e)
            {
                model.Status = new StatusMessage
                {
                    Type = StatusMessage.Error,
                    Body = e.Message
                };

                return model;
            }
            /*
            catch
            {
                return new StatusMessage
                {
                    Type = StatusMessage.Error,
                    Body = "An error occured while saving the page"
                };
            }
            */

            var ret = await _service.GetById(model.Id);
            ret.Status = new StatusMessage
            {
                Type = StatusMessage.Success,
                Body = draft ? _localizer.Post["The post was successfully saved"]
                    : string.IsNullOrEmpty(model.Published) ? _localizer.Post["The post was successfully unpublished"] : _localizer.Page["The post was successfully published"]
            };

            return ret;
        }
    }
}