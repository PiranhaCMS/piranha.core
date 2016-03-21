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
using Microsoft.AspNet.Mvc;
using Piranha;

namespace Blog.Controllers
{
	/// <summary>
	/// Just a basic controller for handling the Piranha content.
	/// </summary>
    public class BlogController : Controller
    {
		/// <summary>
		/// Gets the page with the specified id.
		/// </summary>
		/// <param name="id">The unique id</param>
		[Route("page")]
		public IActionResult Page(Guid id) {
			using (var api = new Api()) {
				return View(api.Pages.GetById(id));
			}
		}

		/// <summary>
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="id">The unique id</param>
		[Route("post")]
		public IActionResult Post(Guid id) {
			using (var api = new Api()) {
				return View(api.Posts.GetById(id));
			}
		}
	}
}
