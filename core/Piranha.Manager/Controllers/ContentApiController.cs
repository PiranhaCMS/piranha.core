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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Manager.Models;
using Piranha.Manager.Models.Content;
using Piranha.Manager.Services;

namespace Piranha.Manager.Controllers
{
    /// <summary>
    /// Api controller for content management.
    /// </summary>
    [Area("Manager")]
    [Route("manager/api/content")]
    [Authorize(Policy = Permission.Admin)]
    [ApiController]
    public class ContentApiController : Controller
    {
        private readonly ContentTypeService _service;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ContentApiController(ContentTypeService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets the currently available block types.
        /// </summary>
        /// <param name="parentType">The optional parent group type</param>
        /// <returns>The block list model</returns>
        [Route("blocktypes/{parentType?}")]
        [HttpGet]
        public BlockListModel GetBlockTypes(string parentType = null)
        {
            return _service.GetBlockTypes(parentType);
        }

        /// <summary>
        /// Creates a new block of the specified type.
        /// </summary>
        /// <param name="type">The block type</param>
        /// <returns>The new block</returns>
        [Route("block/{type}")]
        [HttpGet]
        public IActionResult CreateBlock(string type)
        {
            var block = _service.CreateBlock(type);

            if (block != null)
            {
                return Ok(block);
            }
            return NotFound();
        }

        [Route("page/region/{type}/{region}")]
        [HttpGet]
        public IActionResult CreatePageRegion(string type, string region)
        {
            return Ok(_service.CreatePageRegion(type, region));
        }
    }
}