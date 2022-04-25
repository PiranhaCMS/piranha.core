/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Piranha.Manager.Models
{
    [Authorize(Policy = Permission.Admin)]
    public class IndexModel : PageModel
    {
        private readonly IAuthorizationService _service;

        public IndexModel(IAuthorizationService service)
        {
            _service = service;
        }
        public async Task<IActionResult> OnGet(string returnUrl = null)
        {
            var items = await Menu.Items.GetForUser(HttpContext.User, _service);

            if (items.Count > 0)
            {
                return Redirect(items[0].Items[0].Route);
            }
            return RedirectToPage("Logout");
        }
    }
}