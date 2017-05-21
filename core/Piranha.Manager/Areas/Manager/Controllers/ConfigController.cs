/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Piranha.Models;
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
        public ConfigController(Api api) : base(api) { }

        /// <summary>
        /// Gets the config view.
        /// </summary>
        [Route("manager/config")]
        public IActionResult Edit() {
            return View(Models.ConfigEditModel.Get(api));
        }

        /// <summary>
        /// Saves the config
        /// </summary>
        [HttpPost]
        [Route("manager/config/save")]
        public IActionResult Save(Models.ConfigEditModel model) {
            model.Save(api);

            SuccessMessage("Updated the configuration.");

            return RedirectToAction("Edit");
        }
    }
}