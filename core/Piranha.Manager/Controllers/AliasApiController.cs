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
    /// <summary>
    /// Api controller for alias management.
    /// </summary>
    [Area("Manager")]
    [Route("manager/api/alias")]
    [Authorize(Policy = Permission.Admin)]
    [ApiController]
    public class AliasApiController : Controller
    {
        private readonly IApi _api;
        private readonly AliasService _service;
        private readonly ManagerLocalizer _localizer;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AliasApiController(IApi api, AliasService service, ManagerLocalizer localizer)
        {
            _api = api;
            _service = service;
            _localizer = localizer;
        }

        /// <summary>
        /// Gets the list model.
        /// </summary>
        /// <returns>The list model</returns>
        [Route("list/{siteId?}")]
        [HttpGet]
        [Authorize(Policy = Permission.Aliases)]
        public async Task<AliasListModel> List(Guid? siteId = null)
        {
            return await _service.GetList(siteId);
        }

        /// <summary>
        /// Saves the given alias and returns the updated list model.
        /// </summary>
        /// <param name="model">The alias</param>
        /// <returns>The updated list model</returns>
        [Route("save")]
        [HttpPost]
        [Authorize(Policy = Permission.AliasesEdit)]
        public async Task<IActionResult> Save(AliasListModel.ListItem model)
        {
            try
            {
                await _service.Save(model);

                var result = await _service.GetList(model.SiteId);

                result.Status = new StatusMessage
                {
                    Type = StatusMessage.Success,
                    Body = _localizer.Alias["The alias was successfully added to the list"]
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
        [Authorize(Policy = Permission.AliasesDelete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var alias = await _service.Delete(id);

            if (alias != null)
            {
                var result = await _service.GetList(alias.SiteId);

                result.Status = new StatusMessage
                {
                    Type = StatusMessage.Success,
                    Body = _localizer.Alias["The alias was successfully deleted"]
                };

                return Ok(result);
            }
            return NotFound();
        }
    }
}