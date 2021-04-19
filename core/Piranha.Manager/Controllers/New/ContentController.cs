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
    [Authorize(Policy = Permission.Admin)]
    public sealed class ContentController : Controller
    {
        private readonly ContentServiceLabs _service;
        private readonly ManagerLocalizer _localizer;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="service">The content service</param>
        /// <param name="localizer">The localization service</param>
        public ContentController(ContentServiceLabs service, ManagerLocalizer localizer)
        {
            _service = service;
            _localizer = localizer;
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
        public Task<ContentModel> GetAsync(Guid id, Guid? langId = null)
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
        public async Task<ContentModel> SaveAsync(ContentModel model)
        {
            ContentModel result = null;

            try
            {
                result = await _service.SaveContentAsync(model);
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
                    Body = _localizer.Content["An error occured while saving the content"]
                };
                return model;
            }

            result.Status = new StatusMessage
            {
                Type = StatusMessage.Success,
                Body = _localizer.Content["The content was successfully saved"]
            };
            return result;
        }

        /// <summary>
        /// Deletes the content model with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>If the content was successfully deleted</returns>
        [HttpDelete]
        [Route("content")]
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
                        Body = _localizer.Content["The content was successfully deleted"]
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
                Body = _localizer.Content["An error occured while deleting the content"]
            };
        }
    }
}