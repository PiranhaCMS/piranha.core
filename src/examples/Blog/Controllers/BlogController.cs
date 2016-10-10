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
using System;
using Piranha;

namespace Blog.Controllers
{
    public class BlogController : Controller
    {
		#region Members
		/// <summary>
		/// The private api.
		/// </summary>
		private readonly IApi api;
		#endregion

		/// <summary>
		/// Default construtor.
		/// </summary>
		/// <param name="api">The current api</param>
		public BlogController(IApi api) {
			this.api = api;
		}

		/// <summary>
		/// Gets the page with the specified id.
		/// </summary>
		/// <param name="id">The unique id</param>
		/// <param name="startpage">If this is the site startpage</param>
		[Route("page")]
		public IActionResult Page(Guid id, bool startpage) {
			if (startpage)
				return View("Start", api.Pages.GetById(id));
			return View(api.Pages.GetById(id));
		}

		/// <summary>
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="id">The unique id</param>
		[Route("post")]
		public IActionResult Post(Guid id) {
			return View(api.Posts.GetById(id));
		}

		/// <summary>
		/// Disposes the controller.
		/// </summary>
		protected override void Dispose(bool disposing) {
			if (disposing && api != null)
				api.Dispose();

			base.Dispose(disposing);
		}
	}
}
