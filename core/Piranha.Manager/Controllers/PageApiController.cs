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
    [Route("manager/api/page")]
    [ApiController]
    public class PageApiController : Controller
    {
        private readonly PageService _service;
        private readonly ManagerLocalizer _localizer;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PageApiController(PageService service, ManagerLocalizer localizer)
        {
            _service = service;
            _localizer = localizer;
        }

        /// <summary>
        /// Gets the list model.
        /// </summary>
        /// <returns>The list model</returns>
        [Route("list")]
        [HttpGet]
        public async Task<PageListModel> List()
        {
            return await _service.GetList();
        }

        /// <summary>
        /// Gets the page with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The page edit model</returns>
        [Route("{id:Guid}")]
        [HttpGet]
        public async Task<PageEditModel> Get(Guid id)
        {
            return await _service.GetById(id);
        }

        /// <summary>
        /// Creates a new page of the specified type.
        /// </summary>
        /// <param name="typeId">The type id</param>
        /// <returns>The page edit model</returns>
        [Route("create/{typeId}")]
        [HttpGet]
        public async Task<PageEditModel> Create(string typeId)
        {
            return await _service.Create(typeId);
        }


        /// <summary>
        /// Saves the given model
        /// </summary>
        /// <param name="model">The model</param>
        /// <returns>The result of the operation</returns>
        [Route("save")]
        [HttpPost]
        public Task<PageEditModel> Save(PageEditModel model)
        {
            // Ensure that we have a published date
            if (string.IsNullOrEmpty(model.Published))
            {
                model.Published = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            }

            return Save(model, false);
        }

        /// <summary>
        /// Saves the given model
        /// </summary>
        /// <param name="model">The model</param>
        /// <returns>The result of the operation</returns>
        [Route("save/draft")]
        [HttpPost]
        public Task<PageEditModel> SaveDraft(PageEditModel model)
        {
            return Save(model, true);
        }

        /// <summary>
        /// Saves the given model and unpublishes it
        /// </summary>
        /// <param name="model">The model</param>
        /// <returns>The result of the operation</returns>
        [Route("save/unpublish")]
        [HttpPost]
        public Task<PageEditModel> SaveUnpublish(PageEditModel model)
        {
            // Remove published date
            model.Published = null;

            return Save(model, false);
        }

        [Route("revert/{id}")]
        [HttpGet]
        public async Task<PageEditModel> Revert(Guid id)
        {
            var page = await _service.GetById(id, false);

            if (page != null)
            {
                await _service.Save(page, false);

                page = await _service.GetById(id);
            }

            page.Status = new StatusMessage
            {
                Type = StatusMessage.Success,
                Body = _localizer.Page["The page was successfully reverted to its previous state"]
            };

            return page;
        }

        /// <summary>
        /// Deletes the page with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The result of the operation</returns>
        [Route("delete/{id}")]
        [HttpGet]
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
                    Body = _localizer.Page["An error occured while deleting the page"]
                };
            }

            return new StatusMessage
            {
                Type = StatusMessage.Success,
                Body = _localizer.Page["The page was successfully deleted"]
            };
        }

        /// <summary>
        /// Saves the given model
        /// </summary>
        /// <param name="model">The model</param>
        /// <returns>The result of the operation</returns>
        private async Task<PageEditModel> Save(PageEditModel model, bool draft = false)
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
                Body = draft ? _localizer.Page["The page was successfully saved"]
                    : string.IsNullOrEmpty(model.Published) ? _localizer.Page["The page was successfully unpublished"] : _localizer.Page["The page was successfully published"]
            };

            return ret;
        }
    }
}