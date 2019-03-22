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
    [Area("Manager")]
    [Route("manager")]
    public class HomeController : Controller
    {
        private readonly IApi _api;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public HomeController(IApi api)
        {
            _api = api;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}