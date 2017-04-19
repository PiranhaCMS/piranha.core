/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.AspNetCore.Mvc;

namespace Piranha.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class StartController : Controller
    {
        /// <summary>
        /// Redirects to the current startpage of the
        /// manager area.
        /// </summary>
        [Route("manager")]
        public IActionResult Index() {
            return RedirectToAction("List", "Page");
        }
    }
}
