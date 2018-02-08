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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;

namespace Piranha.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class AliasController : ManagerAreaControllerBase 
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public AliasController(IApi api) : base(api) { }

        /// <summary>
        /// Gets the alias view.
        /// </summary>
        [Route("manager/aliases/{siteId:Guid?}")]
        [Authorize(Policy = Permission.Aliases)]
        public IActionResult List(Guid? siteId = null) {
            return View(Models.AliasEditModel.Get(api, siteId));
        }
    }
}
 