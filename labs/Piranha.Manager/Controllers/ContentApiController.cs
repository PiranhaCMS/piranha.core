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
                    Model = (Extend.Block)_factory.CreateBlock(type)
                };

                return Ok(block);
            }
            return NotFound();
        }

        [Route("blocktypes/{parentType?}")]
        [HttpGet]
        public BlockListModel GetBlockTypes(string parentType = null)
        {
            var model = new BlockListModel();
            var parent = App.Blocks.GetByType(parentType);
            var exludeGroups = parent != null && typeof(Piranha.Extend.BlockGroup).IsAssignableFrom(parent.Type);

            foreach (var category in App.Blocks.GetCategories())
            {
                var listCategory = new BlockListModel.ListCategory
                {
                    Name = category
                };

                var items = App.Blocks.GetByCategory(category).Where(i => !i.IsUnlisted);

                // If we have a parent, filter on allowed types
                if (parent != null)
                {
                    if (parent.ItemTypes.Count > 0)
                    {
                        items = items.Where(i => parent.ItemTypes.Contains(i.Type));
                    }

                    if (exludeGroups)
                    {
                        items = items.Where(i => !typeof(Piranha.Extend.BlockGroup).IsAssignableFrom(i.Type));
                    }
                }

                foreach (var block in items) {
                    listCategory.Items.Add(new BlockListModel.ListItem
                    {
                        Name = block.Name,
                        Icon = block.Icon,
                        Type = block.TypeName
                    });
                }
                model.Categories.Add(listCategory);
            }

            // Remove empty categories
            var empty = model.Categories.Where(c =>  c.Items.Count() == 0).ToList();
            foreach (var remove in empty)
            {
                model.Categories.Remove(remove);
            }

            // Calculate type count
            model.TypeCount = model.Categories.Sum(c => c.Items.Count());

            return model;
        }
    }
}