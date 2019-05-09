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
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Piranha.Manager.Models;
using Piranha.Manager.Services;

namespace Piranha.Manager.Controllers
{
    /// <summary>
    /// Api controller for alias management.
    /// </summary>
    [Area("Manager")]
    [Route("manager/api/alias")]
    [ApiController]
    public class AliasApiController : Controller
    {
        private readonly IApi _api;
        private readonly AliasService _service;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AliasApiController(IApi api, AliasService service)
        {
            _api = api;
            _service = service;
        }

        /// <summary>
        /// Gets the list model.
        /// </summary>
        /// <returns>The list model</returns>
        [Route("list")]
        [HttpGet]
        public async Task<AliasListModel> List()
        {
            return await _service.GetList();
        }

        /// <summary>
        /// Saves the given alias and returns the updated list model.
        /// </summary>
        /// <param name="model">The alias</param>
        /// <returns>The updated list model</returns>
        [Route("save")]
        [HttpPost]
        public async Task<IActionResult> Save(AliasListModel.ListItem model)
        {
            try
            {
                await _service.Save(model);

                var result = await _service.GetList();

                result.Status = new StatusMessage
                {
                    Type = StatusMessage.Success,
                    Body = $"The alias <code>{ model.AliasUrl }</code> was added to the list"
                };

                return Ok(result);
            }
            catch (ValidationException e)
            {
                var result = new AliasListModel();
                result.Status = new StatusMessage
                {
                    Type = StatusMessage.Error,
                    Body = e.Message
                };
                return BadRequest(result);
            }
        }

        /// <summary>
        /// Deletes the alias with the given id and returns the updated
        /// list model.
        /// </summary>
        /// <param name="id">The alias id</param>
        /// <returns>The updated list model</returns>
        [Route("delete/{id:Guid}")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var alias = await _service.Delete(id);

            if (alias != null)
            {
                var result = await _service.GetList(alias.SiteId);

                result.Status = new StatusMessage
                {
                    Type = StatusMessage.Success,
                    Body = $"The alias <code>{ alias.AliasUrl }</code> was deleted"
                };

                return Ok(result);
            }
            return NotFound();
        }
    }
}