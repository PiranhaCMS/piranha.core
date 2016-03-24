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
		public BlogController(Api api) {
			this.api = api;
		}

		/// <summary>
		/// Gets the page with the specified id.
		/// </summary>
		/// <param name="id">The unique id</param>
		/// <param name="startpage">If this is the site startpage</param>
		[Route("page")]
		public IActionResult Page(Guid id, bool startpage) {
			if (startpage) {
				var model = api.Pages.GetById<Models.StartModel>(id);
				var cat = api.Categories.GetBySlug("blog");

				model.Archive = api.Archives.GetByCategoryId(cat.Id);

				return View("Start", model);
			}
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
