/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Manager.Models;
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
        public async Task<IActionResult> CreateBlockAsync(string type)
        {
            var block = await _service.CreateBlockAsync(type);

            if (block != null)
            {
                return Ok(block);
            }
            return NotFound();
        }

        /// <summary>
        /// Creates a new region for the specified content type.
        /// </summary>
        /// <param name="content">The type of content</param>
        /// <param name="type">The content type</param>
        /// <param name="region">The region id</param>
        /// <returns>The new region model</returns>
        [Route("region/{content}/{type}/{region}")]
        [HttpGet]
        public async Task<IActionResult> CreateRegionAsync(string content, string type, string region)
        {
            if (content == "page")
            {
                return Ok(await _service.CreatePageRegionAsync(type, region));
            }
            else if (content == "post")
            {
                return Ok(await _service.CreatePostRegionAsync(type, region));
            }
            else if (content == "site")
            {
                return Ok(await _service.CreateSiteRegionAsync(type, region));
            }
            return NotFound();
        }
    }
}