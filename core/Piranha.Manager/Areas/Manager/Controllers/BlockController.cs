/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Mvc;
using Piranha.Services;

namespace Piranha.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class BlockController : ManagerAreaControllerBase
    {
        private readonly IContentFactory _factory;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="factory">The content factory</param>
        public BlockController(IApi api, IContentFactory factory) : base(api)
        {
            _factory = factory;
        }

        /// <summary>
        /// Creates a new block edit view.
        /// </summary>
        /// <param name="model">The model</param>
        [HttpPost]
        [Route("manager/block/create")]
        public IActionResult AddBlock([FromBody]Models.ContentBlockModel model)
        {
            var block = (Extend.Block)_factory.CreateBlock(model.TypeName);

            if (block != null)
            {
                ViewBag.IsInGroup = !model.IncludeGroups;
                ViewBag.GroupType = !string.IsNullOrEmpty(model.GroupType) ? App.Blocks.GetByType(model.GroupType) : null;
                if (model.IncludeGroups)
                    ViewData.TemplateInfo.HtmlFieldPrefix = $"Blocks[{model.BlockIndex}]";
                else ViewData.TemplateInfo.HtmlFieldPrefix = $"Blocks[{model.BlockIndex}].Items[0]";
                return View("EditorTemplates/ContentEditBlock", new Models.ContentEditBlock
                {
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
