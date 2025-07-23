/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Piranha.Manager.Models
{
    [Authorize(Policy = Security.Permission.PagePreview)]
    public class PagePreviewModel : PageModel
    {
        private readonly IApi _api;
        public Guid Id { get; set; }
        public string Permalink { get; set; }

        public PagePreviewModel(IApi api)
        {
            _api = api;
        }

        public async Task<IActionResult> OnGet(Guid id)
        {
            var page = await _api.Pages.GetByIdAsync<Piranha.Models.PageInfo>(id);

            if (page == null)
            {
                return NotFound();
            }

            Id = page.Id;
            Permalink = page.Permalink + "?draft=true";

            return Page();
        }
    }
}