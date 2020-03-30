/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

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
    [Route("manager/api/config")]
    [Authorize(Policy = Permission.Admin)]
    [ApiController]
    public class ConfigApiController : Controller
    {
        private readonly ConfigService _service;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConfigApiController(ConfigService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets the list model.
        /// </summary>
        /// <returns>The list model</returns>
        [Route("")]
        [HttpGet]
        [Authorize(Policy = Permission.Config)]
        public ConfigModel List()
        {
            return _service.Get();
        }

        /// <summary>
        /// Save the given model.
        /// </summary>
        /// <param name="model">The config model</param>
        [Route("save")]
        [HttpPost]
        [Authorize(Policy = Permission.ConfigEdit)]
        public AsyncResult Save(ConfigModel model)
        {
            try
            {
                _service.Save(model);
            }
            catch
            {
                return new AsyncResult
                {
                    Status = new StatusMessage
                    {
                        Type = StatusMessage.Error,
                        Body = "An error occurred while saving"
                    }
                };
            }
            return new AsyncResult
            {
                Status = new StatusMessage
                {
                    Type = StatusMessage.Success,
                    Body = "The config was successfully saved"
                }
            };
        }
    }
}