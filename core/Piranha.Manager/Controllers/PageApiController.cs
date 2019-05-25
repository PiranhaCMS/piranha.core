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

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PageApiController(PageService service)
        {
            _service = service;
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
        /// Gets the sitemap model.
        /// </summary>
        /// <returns>The list model</returns>
        [Route("sitemap")]
        [HttpGet]
        public async Task<Sitemap> Sitemap()
        {
            return await _service.GetSitemap();
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
        public async Task<StatusMessage> Save(PageEditModel model)
        {
            try
            {
                await _service.Save(model);
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

            return new StatusMessage
            {
                Type = StatusMessage.Success,
                Body = "The page was successfully saved"
            };
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
                    Body = "An error occured while deleting the page"
                };
            }

            return new StatusMessage
            {
                Type = StatusMessage.Success,
                Body = "The page was successfully deleted"
            };
        }
    }
}