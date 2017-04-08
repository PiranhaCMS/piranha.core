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
using System;
using Piranha;

namespace CoreWeb.Controllers
{
    public class CmsController : Controller
    {
        #region Members
        /// <summary>
        /// The private api.
        /// </summary>
        private readonly Api api;
        #endregion

        /// <summary>
        /// Default construtor.
        /// </summary>
        /// <param name="api">The current api</param>
        public CmsController(Api api) {
            this.api = api;
        }

        /// <summary>
        /// Gets the page with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="startpage">If this is the site startpage</param>
        [Route("page")]
        public IActionResult Page(string id, bool startpage) {
            var model = api.Pages.GetById<Models.MarkdownPage>(id);
            ViewBag.CurrentPage = model.Id;

            if (startpage)
                return View("Start", model);
            return View(model);
        }
    }
}
