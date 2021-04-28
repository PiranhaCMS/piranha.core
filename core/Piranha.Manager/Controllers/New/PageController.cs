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
using Microsoft.Extensions.Logging;
using Piranha.Manager.Hubs;
using Piranha.Manager.Models;
using Piranha.Manager.Services;

namespace Piranha.Manager.Controllers
{
    [Area("Manager")]
    [Route("manager/api/labs")]
    [ApiController]
    [Authorize(Policy = Permission.Admin)]
    public sealed class PageController : Controller
    {
        private readonly ContentServiceLabs _service;
        private readonly ManagerLocalizer _localizer;
        private readonly IHubContext<PreviewHub> _hub;
        private readonly ILogger _logger;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="service">The content service</param>
        /// <param name="localizer">The localization service</param>
        /// <param name="hub">The SignalR preview hub</param>
        /// <param name="factory">The optional logger factory</param>
        public PageController(ContentServiceLabs service, ManagerLocalizer localizer, IHubContext<PreviewHub> hub,
            ILoggerFactory factory = null)
        {
            _service = service;
            _localizer = localizer;
            _hub = hub;
            _logger = factory?.CreateLogger(typeof(PageController));
        }

        /// <summary>
        /// Gets the content model for the page with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The content model</returns>
        [HttpGet]
        [Route("page/{id}")]
        [Authorize(Policy = Permission.PagesEdit)]
        public Task<ContentModel> GetAsync(Guid id)
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
        public Task<ContentModel> SaveAsync(ContentModel model)
        {
            return SaveAsync(model, false);
        }

        /// <summary>
        /// Saves the given page as a draft.
        /// </summary>
        /// <param name="model">The content model</param>
        /// <returns>The updated content model</returns>
        [HttpPost]
        [Route("page/draft")]
        [Authorize(Policy = Permission.PagesSave)]
        public Task<ContentModel> SaveDraftAsync(ContentModel model)
        {
            return SaveAsync(model, true);
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
        public async Task<ContentModel> RevertAsync([FromBody]Guid id)
        {
            var model = await _service.RevertPageAsync(id);

            model.Status = new StatusMessage
            {
                Type = StatusMessage.Success,
                Body = _localizer.Page["The page was successfully reverted to its previous state"]
            };

            await _hub?.Clients.All.SendAsync("Update", id);

            return model;
        }

        /// <summary>
        /// Unpublishes the page with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The updated content model</returns>
        [HttpPost]
        [Route("page/unpublish")]
        [Authorize(Policy = Permission.PagesSave)]
        public async Task<ContentModel> UnpublishAsync([FromBody]Guid id)
        {
            var model = await _service.UnpublishPageAsync(id);

            model.Status = new StatusMessage
            {
                Type = StatusMessage.Success,
                Body = _localizer.Page["The page was successfully unpublished"]
            };

            await _hub?.Clients.All.SendAsync("Update", id);

            return model;
        }

        /// <summary>
        /// Deletes the page model with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>If the page was successfully deleted</returns>
        [HttpDelete]
        [Route("page")]
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
                        Body = _localizer.Page["The page was successfully deleted"]
                    };
                }
            }
            catch (ValidationException e)
            {
                // Log the exception
                _logger?.LogWarning(e.Message);

                // Validation did not succeed, return the
                // validation error
                return new StatusMessage
                {
                    Type = StatusMessage.Error,
                    Body = e.Message
                };
            }
            catch (Exception e)
            {
                // Log the exception
                _logger?.LogError(e.Message);
            }

            // The operation failed. Return generic error message.
            return new StatusMessage
            {
                Type = StatusMessage.Error,
                Body = _localizer.Page["An error occured while deleting the page"]
            };
        }

        /// <summary>
        /// Saves the given page.
        /// </summary>
        /// <param name="model">The content model</param>
        /// <param name="draft">If the page should be saved as a draft</param>
        /// <returns>The updated content model</returns>
        private async Task<ContentModel> SaveAsync(ContentModel model, bool draft)
        {
            ContentModel result = null;

            try
            {
                result = await _service.SavePageAsync(model, draft);
            }
            catch (ValidationException e)
            {
                // Log the exception
                _logger?.LogWarning(e.Message);

                model.Status = new StatusMessage
                {
                    Type = StatusMessage.Error,
                    Body = e.Message
                };
                return model;
            }
            catch (Exception e)
            {
                // Log the exception
                _logger?.LogError(e.Message);

                model.Status = new StatusMessage
                {
                    Type = StatusMessage.Error,
                    Body = _localizer.Page["An error occured while saving the page"]
                };
                return model;
            }

            result.Status = new StatusMessage
            {
                Type = StatusMessage.Success,
                Body = draft ? _localizer.Page["The page was successfully saved"] : _localizer.Page["The page was successfully published"]
            };
            return result;
        }
    }
}