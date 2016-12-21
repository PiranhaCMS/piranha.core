/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using Microsoft.AspNetCore.Mvc;

namespace Piranha.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class PostController : ManagerAreaControllerBase
    {
        /// <summary>
        /// Default constroller.
        /// </summary>
        /// <param name="api">The current api</param>
        public PostController(IApi api) : base(api) { }

        /// <summary>
        /// Gets the list view for the posts.
        /// </summary>
        /// <param name="category">The optional category slug</param>
        [Route("manager/posts/{category?}")]
        public IActionResult List(string category = null) {
            return View(Models.PostListModel.Get(api, category));
        }

        [Route("manager/post/{id:Guid}")]
        public IActionResult Edit(Guid id) {
            return View(Models.PostEditModel.GetById(api, id));
        }
    }
}
