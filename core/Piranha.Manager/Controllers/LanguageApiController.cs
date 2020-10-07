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
using Piranha.Models;

namespace Piranha.Manager.Controllers
{
    /// <summary>
    /// Api controller for alias management.
    /// </summary>
    [Area("Manager")]
    [Route("manager/api/language")]
    [Authorize(Policy = Permission.Admin)]
    [ApiController]
    public class LanguageApiController : Controller
    {
        private readonly LanguageService _service;
        private readonly ManagerLocalizer _localizer;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LanguageApiController(LanguageService service, ManagerLocalizer localizer)
        {
            _service = service;
            _localizer = localizer;
        }

        /// <summary>
        /// Gets the edit model.
        /// </summary>
        /// <returns>The edit model</returns>
        [Route("")]
        [HttpGet]
        public async Task<LanguageEditModel> Get()
        {
            return await _service.Get();
        }

        /// <summary>
        /// Saves the edit model.
        /// </summary>
        /// <param name="model">The model</param>
        [Route("")]
        [HttpPost]
        public async Task<LanguageEditModel> Save(LanguageEditModel model)
        {
            return await _service.Save(model);
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<LanguageEditModel> Delete(Guid id)
        {
            return await _service.Delete(id);
        }
    }
}