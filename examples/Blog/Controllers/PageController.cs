/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.dnx
 * 
 */

using System;
using System.Linq;
using Microsoft.AspNet.Mvc;
using Piranha;

namespace Blog.Controllers
{
	/// <summary>
	/// Default application controller.
	/// </summary>
	public class PageController : Controller
	{
		/// <summary>
		/// Gets the startpage of the blog.
		/// </summary>
		public IActionResult Index() {
			if (Request.Query["id"].Count == 1) {
				try {
					var id = new Guid(Request.Query["id"][0]);
					using (var api = new Api()) {
						return View(api.Pages.GetById(id));
					}
				} catch { }
			}
			return null;
		}
	}
}
