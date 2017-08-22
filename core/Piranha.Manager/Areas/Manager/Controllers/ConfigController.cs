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
    public class ConfigController : ManagerAreaControllerBase
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public ConfigController(IApi api) : base(api) { }

        /// <summary>
        /// Gets the config view.
        /// </summary>
        [Route("manager/config")]
        [Authorize(Policy = Permission.Config)]
        public IActionResult Edit() {
            return View(Models.ConfigEditModel.Get(api));
        }

        /// <summary>
        /// Saves the config
        /// </summary>
        [HttpPost]
        [Route("manager/config/save")]
        [Authorize(Policy = Permission.ConfigEdit)]
        public IActionResult Save(Models.ConfigEditModel model) {
            model.Save(api);

            SuccessMessage("Updated the configuration.");

            return RedirectToAction("Edit");
        }
    }
}