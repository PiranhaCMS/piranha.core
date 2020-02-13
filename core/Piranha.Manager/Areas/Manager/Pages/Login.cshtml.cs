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

        public bool SignInFailed { get; set; }

        public LoginModel(ISecurity service)
        {
            _service = service;
        }

        public class InputModel
        {
            [Required]
            public string Username { get; set; }

            [Required]
            public string Password { get; set; }
        }

        public async Task<IActionResult> OnPostAsync(InputModel model)
        {            
            if (ModelState.IsValid && await _service.SignIn(HttpContext, model.Username, model.Password))
                return new RedirectToPageResult("Index");

            SignInFailed = true;
            return Page();
        }
    }
}