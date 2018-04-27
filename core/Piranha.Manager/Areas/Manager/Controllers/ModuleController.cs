/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Piranha.Manager;
using Piranha.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;

namespace Piranha.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class ModuleController : ManagerAreaControllerBase
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public ModuleController(IApi api) : base(api) { }

        /// <summary>
        /// Gets the about view.
        /// </summary>
        [Route("manager/modules")]
        [Authorize(Policy = Permission.Config)]
        public IActionResult List() {
            return View("List", Models.ModuleListModel.Get());
        }
    }
}