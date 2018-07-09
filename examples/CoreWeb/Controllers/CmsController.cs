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
using System;

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
        /// Gets the archive for the category with the specified id.
        /// </summary>
        /// <param name="id">The category id</param>
        /// <param name="year">The optional year</param>
        /// <param name="month">The optional month</param>
        /// <param name="page">The optional page</param>
        /// <param name="category">The optional category id</param>
        [Route("archive")]
        public IActionResult Archive(Guid id, int? year = null, int? month = null, int? page = null, Guid? category = null, Guid? tag = null) {
            Models.BlogArchive model;

            if (category.HasValue)
                model = api.Archives.GetByCategoryId<Models.BlogArchive>(id, category.Value, page, year, month);
            else if (tag.HasValue)
                model = api.Archives.GetByTagId<Models.BlogArchive>(id, tag.Value, page, year, month);
            else model = api.Archives.GetById<Models.BlogArchive>(id, page, year, month);

            return View(model);
        }

        /// <summary>
        /// Gets the page with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        [Route("page")]
        public IActionResult Page(Guid id) {
            var model = api.Pages.GetById<Models.StandardPage>(id);

            return View(model);
        }

        /// <summary>
        /// Gets the post with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        [Route("post")]
        public IActionResult Post(Guid id) {
            var model = api.Posts.GetById<Models.BlogPost>(id);

            return View(model);
        }

        /// <summary>
        /// Gets the page with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="startpage">If this is the site startpage</param>
        [Route("teaserpage")]
        public IActionResult TeaserPage(Guid id, bool startpage) {
            var model = api.Pages.GetById<Models.TeaserPage>(id);

            if (startpage)
                return View("Start", model);
            return View(model);
        }
    }
}
