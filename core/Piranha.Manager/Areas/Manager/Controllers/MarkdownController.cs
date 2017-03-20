/*
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
    public class MarkdownController : Controller
    {
        /// <summary>
        /// Transforms the given markdown string to HTML.
        /// </summary>
        /// <param name="str">The markdown string</param>
        /// <returns>The HTML value</returns>
        [HttpPost]
        [Route("manager/markdown")]
        public IActionResult Transform([FromBody]string str) {
            return Json(new {
                Body = App.Markdown.Transform(str)
            });
        }
    }
}
