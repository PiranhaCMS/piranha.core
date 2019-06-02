/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Piranha.Manager.Models
{
    public class LogoutModel : PageModel
    {
        private readonly ISecurity _service;

        public LogoutModel(ISecurity service)
        {
            _service = service;
        }

        public async Task<IActionResult> OnGet()
        {
            await _service.SignOut(HttpContext);

            return new RedirectToPageResult("Login");
        }
    }
}