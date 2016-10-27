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
    public class PostController : Controller
    {
        #region Members
        /// <summary>
        /// The current api.
        /// </summary>
        private IApi api;
        #endregion

        /// <summary>
        /// Default constroller.
        /// </summary>
        /// <param name="api">The current api</param>
        public PostController(IApi api) {
            this.api = api;
        }

        /// <summary>
        /// Gets the list view for the posts.
        /// </summary>
        [Route("manager/posts")]
        public IActionResult List() {
            return View();
        }

        /// <summary>
        /// Disposes the controller and its resources.
        /// </summary>
        protected override void Dispose(bool disposing) {
            api.Dispose();

            base.Dispose(disposing);
        }
    }
}
