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
using Microsoft.AspNetCore.Mvc;
using Piranha.Manager.Models;
using Piranha.Services;

namespace Piranha.Manager.Controllers
{
    /// <summary>
    /// Api controller for content management.
    /// </summary>
    [Area("Manager")]
    [Route("manager/api/content")]
    [ApiController]
    public class ContentApiController : Controller
    {
        private readonly IContentFactory _factory;
        private List<Tuple<string, string>> _map = new List<Tuple<string, string>>
        {
            new Tuple<string, string>("Piranha.Extend.Blocks.HtmlBlock", "html-block"),
            new Tuple<string, string>("Piranha.Extend.Blocks.HtmlColumnBlock", "html-column-block"),
            new Tuple<string, string>("Piranha.Extend.Blocks.ImageBlock", "image-block"),
            new Tuple<string, string>("Piranha.Extend.Blocks.TextBlock", "text-block")
        };

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ContentApiController(IContentFactory factory)
        {
            _factory = factory;
        }

        [Route("block/{type}")]
        [HttpGet]
        public IActionResult CreateBlock(string type)
        {
            var blockType = App.Blocks.GetByType(type);
            var componentType = _map.FirstOrDefault(t => t.Item1 == type)?.Item2;

            if (blockType != null)
            {
                var block = new ContentEditModel.BlockItem
                {
                    Name = blockType.Name,
                    Icon = blockType.Icon,
                    Component = !string.IsNullOrEmpty(componentType) ? componentType : "missing-block",
                    Item = (Extend.Block)_factory.CreateBlock(type)
                };

                return Ok(block);
            }
            return NotFound();
        }
    }
}