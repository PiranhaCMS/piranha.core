/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Piranha.Areas.Manager.Controllers
{
    public class BlockTypeController : ManagerAreaControllerBase
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public BlockTypeController(IApi api) : base(api)
        {
        }

        /// <summary>
        /// Gets the list view for the block types.
        /// </summary>
        [Route("manager/blocktypes")]
        public IActionResult List() {
            return View(App.BlockTypes);
        }


        /// <summary>
        /// Gets the edit view for the specified block type.
        /// </summary>
        [Route("manager/blocktype/{id}")]
        public IActionResult Edit(string id) {
            return View(App.BlockTypes.SingleOrDefault(t => t.Id.ToLower() == id.ToLower()));
        }
    }
}
