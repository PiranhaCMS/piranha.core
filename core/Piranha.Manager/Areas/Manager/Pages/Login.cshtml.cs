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
    public class LoginModel : PageModel
    {
        private readonly ISecurity _service;
        private readonly ManagerLocalizer _localizer;

        public LoginModel(ISecurity service, ManagerLocalizer localizer)
        {
            _service = service;
            _localizer = localizer;
        }

        public class InputModel
        {
            public string Username { get; set; }

            public string Password { get; set; }
        }

        public async Task<IActionResult> OnPostAsync(InputModel model)
        {
            if (string.IsNullOrEmpty(model.Password))
                ModelState.AddModelError("Password", _localizer.General["The password is required."].Value);
            if (string.IsNullOrEmpty(model.Username))
                ModelState.AddModelError("Username", _localizer.General["The username is required."].Value);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (await _service.SignIn(HttpContext, model.Username, model.Password))
            {
                return new RedirectToPageResult("Index");
            }
            ModelState.AddModelError(string.Empty, _localizer.General["Invalid login attempt."].Value);
            return Page();
        }
    }
}