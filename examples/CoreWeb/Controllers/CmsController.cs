/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/tidyui/coreweb
 * 
 */

using Microsoft.AspNetCore.Mvc;
using Piranha;

namespace CoreWeb.Controllers
{
    /// <summary>
    /// Simple controller for handling the CMS content from Piranha.
    /// </summary>
    public class CmsController : Controller
    {
        /// <summary>
        /// The private api.
        /// </summary>
        private readonly IApi api;

        /// <summary>
        /// Default construtor.
        /// </summary>
        /// <param name="api">The current api</param>
        public CmsController(IApi api) {
            this.api = api;
        }

        /// <summary>
        /// Gets the page with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        [Route("page")]
        public IActionResult Page(string id) {
            var model = api.Pages.GetById<Models.StandardPage>(id);
            ViewBag.CurrentPage = model.Id;

            return View(model);
        }

        /// <summary>
        /// Gets the page with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="startpage">If this is the site startpage</param>
        [Route("teaserpage")]
        public IActionResult TeaserPage(string id, bool startpage) {
            var model = api.Pages.GetById<Models.TeaserPage>(id);
            ViewBag.CurrentPage = model.Id;

            if (startpage)
                return View("Start", model);
            return View(model);
        }
    }
}
