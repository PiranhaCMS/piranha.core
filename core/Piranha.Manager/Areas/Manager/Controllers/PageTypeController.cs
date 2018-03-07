﻿/*
 * Copyright (c) 2016-2017 Håkan Edling
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
    public class PageTypeController : ManagerAreaControllerBase
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public PageTypeController(IApi api) : base(api) { }

        /// <summary>
        /// Gets the list view for the page types.
        /// </summary>
        [Route("manager/pagetypes")]
        public IActionResult List()
        {
            return View(Api.PageTypes.GetAll());
        }

        /// <summary>
        /// Gets the edit view for the specified page type.
        /// </summary>
        [Route("manager/pagetype/{id}")]
        public IActionResult Edit(string id)
        {
            return View(Api.PageTypes.GetById(id));
        }
    }
}
