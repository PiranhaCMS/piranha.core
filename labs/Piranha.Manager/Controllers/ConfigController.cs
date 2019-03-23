/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Mvc;

namespace Piranha.Manager.Controllers
{
    /// <summary>
    /// Mvc controller for config management.
    /// </summary>
    [Area("Manager")]
    [Route("manager")]
    public class ConfigController : Controller
    {
        /// <summary>
        /// Gets the config view.
        /// </summary>
        [Route("config")]
        public IActionResult Edit()
        {
            return View();
        }
    }
}