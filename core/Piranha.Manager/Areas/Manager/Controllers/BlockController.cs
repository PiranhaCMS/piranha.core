/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Piranha.Manager;
using Piranha.Models;
using Piranha.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;

namespace Piranha.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class BlockController : ManagerAreaControllerBase 
    {
        private readonly IContentService<Data.Page, Data.PageField, Piranha.Models.PageBase> contentService;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="factory">The content service factory</param>
        public BlockController(IApi api, IContentServiceFactory factory) : base(api) { 
            // Block transformation is not dependent on which content
            // type is actually selected, so let's create a page service.
            this.contentService = factory.CreatePageService();            
        }

        /// <summary>
        /// Creates a new block edit view.
        /// </summary>
        /// <param name="model">The model</param>
        [HttpPost]
        [Route("manager/block/create")]
        public IActionResult AddBlock([FromBody]Models.ContentBlockModel model) {
            var block = (Extend.Block)contentService.CreateBlock(model.TypeName);

            if (block != null) {
                ViewBag.IsInGroup = !model.IncludeGroups;
                ViewBag.GroupType = !string.IsNullOrEmpty(model.GroupType) ? App.Blocks.GetByType(model.GroupType) : null;
                if (model.IncludeGroups)
                    ViewData.TemplateInfo.HtmlFieldPrefix = $"Blocks[{model.BlockIndex}]";
                else ViewData.TemplateInfo.HtmlFieldPrefix = $"Blocks[{model.BlockIndex}].Items[0]";
                return View("EditorTemplates/ContentEditBlock", new Models.ContentEditBlock() {
                    Id = block.Id,
                    CLRType = block.GetType().FullName,
                    IsGroup = typeof(Extend.BlockGroup).IsAssignableFrom(block.GetType()),
                    Value = block
                });
            }
            return new NotFoundResult();
        }
    }
}
 