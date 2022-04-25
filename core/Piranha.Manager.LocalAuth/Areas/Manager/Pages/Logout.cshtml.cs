/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Piranha.Manager.LocalAuth.Areas.Manager.Pages
{
    /// <summary>
    /// View model for the logout page.
    /// </summary>
    public class LogoutModel : PageModel
    {
        private readonly ISecurity _service;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="service">The current security service</param>
        public LogoutModel(ISecurity service)
        {
            _service = service;
        }

        /// <summary>
        /// Handles the logout page.
        /// </summary>
        public async Task<IActionResult> OnGet()
        {
            await _service.SignOut(HttpContext);

            return new RedirectToPageResult("Login");
        }
    }
}