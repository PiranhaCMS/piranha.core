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
using Piranha.Manager.Hubs;
using Piranha.Manager.Models;
using Piranha.Manager.Services;

namespace Piranha.Manager.Controllers
{
    [Area("Manager")]
    [Route("manager/api/labs")]
    [ApiController]
    [Authorize(Policy = Permission.Admin)]
    public sealed class PostController : Controller
    {
        private readonly ContentServiceLabs _service;
        private readonly ManagerLocalizer _localizer;
        private readonly IHubContext<PreviewHub> _hub;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="service">The content service</param>
        /// <param name="localizer">The localization service</param>
        /// <param name="hub">The SignalR preview hub</param>
        public PostController(ContentServiceLabs service, ManagerLocalizer localizer, IHubContext<PreviewHub> hub)
        {
            _service = service;
            _localizer = localizer;
            _hub = hub;
        }

        /// <summary>
        /// Gets the content model for the post with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The content model</returns>
        [HttpGet]
        [Route("post/{id}")]
        [Authorize(Policy = Permission.PostsEdit)]
        public Task<ContentModel> GetAsync(Guid id)
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
        public Task<ContentModel> SaveAsync(ContentModel model)
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
        public Task<ContentModel> SaveDraftAsync(ContentModel model)
        {
            return _service.SavePostAsync(model, true);
        }

        /// <summary>
        /// Reverts the post with the given id to its currently
        /// published version.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The reverted content model</returns>
        [HttpPost]
        [Route("post/revert")]
        [Authorize(Policy = Permission.PostsSave)]
        public async Task<ContentModel> RevertAsync([FromBody]Guid id)
        {
            var model = await _service.RevertPostAsync(id);

            model.Status = new StatusMessage
            {
                Type = StatusMessage.Success,
                Body = _localizer.Post["The post was successfully reverted to its previous state"]
            };

            await _hub?.Clients.All.SendAsync("Update", id);

            return model;
        }

        /// <summary>
        /// Unpublishes the post with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The updated content model</returns>
        [HttpPost]
        [Route("post/unpublish")]
        [Authorize(Policy = Permission.PostsPublish)]
        public async Task<ContentModel> UnpublishAsync([FromBody]Guid id)
        {
            var model = await _service.UnpublishPostAsync(id);

            model.Status = new StatusMessage
            {
                Type = StatusMessage.Success,
                Body = _localizer.Post["The post was successfully unpublished"]
            };

            await _hub?.Clients.All.SendAsync("Update", id);

            return model;
        }

        /// <summary>
        /// Deletes the post model with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>If the post was successfully deleted</returns>
        [HttpDelete]
        [Route("post")]
        [Authorize(Policy = Permission.ContentDelete)]
        public async Task<StatusMessage> DeleteAsync([FromBody]Guid id)
        {
            try {
                if (await _service.DeleteContentAsync(id))
                {
                    // Operation succeeded, return success
                    return new StatusMessage
                    {
                        Type = StatusMessage.Success,
                        Body = _localizer.Post["The post was successfully deleted"]
                    };
                }
            }
            catch (ValidationException e)
            {
                // Validation did not succeed, return the
                // validation error
                return new StatusMessage
                {
                    Type = StatusMessage.Error,
                    Body = e.Message
                };
            }
            catch { }

            // The operation failed. Return generic error message.
            return new StatusMessage
            {
                Type = StatusMessage.Error,
                Body = _localizer.Post["An error occured while deleting the post"]
            };
        }

        /// <summary>
        /// Saves the given post.
        /// </summary>
        /// <param name="model">The content model</param>
        /// <param name="draft">If the post should be saved as a draft</param>
        /// <returns>The updated content model</returns>
        private async Task<ContentModel> SaveAsync(ContentModel model, bool draft)
        {
            ContentModel result = null;

            try
            {
                result = await _service.SavePostAsync(model, draft);
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
            catch
            {
                model.Status = new StatusMessage
                {
                    Type = StatusMessage.Error,
                    Body = _localizer.Post["An error occured while saving the post"]
                };
                return model;
            }

            result.Status = new StatusMessage
            {
                Type = StatusMessage.Success,
                Body = draft ? _localizer.Post["The post was successfully saved"] : _localizer.Post["The post was successfully published"]
            };
            return result;
        }
    }
}