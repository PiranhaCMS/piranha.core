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

namespace Piranha.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class PageTypeController : Controller
    {
        #region Members
        /// <summary>
        /// The current api.
        /// </summary>
        private IApi api;
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public PageTypeController(IApi api) {
            this.api = api;
        }

        /// <summary>
        /// Gets the list view for the page types.
        /// </summary>
        [Route("manager/pagetypes")]
        public IActionResult List() {
            return View(App.PageTypes);
        }
    }
}
