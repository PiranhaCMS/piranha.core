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
    }
}