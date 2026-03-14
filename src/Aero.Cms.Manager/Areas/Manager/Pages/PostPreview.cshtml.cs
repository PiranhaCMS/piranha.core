/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/Aerocms/Aero.core
 *
 */

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Aero.Cms.Manager.Models
{
    [Authorize(Policy = Security.Permission.PostPreview)]
    public class PostPreviewModel : PageModel
    {
        private readonly IApi _api;
        public string Id { get; set; }
        public string Permalink { get; set; }

        public PostPreviewModel(IApi api)
        {
            _api = api;
        }

        public async Task<IActionResult> OnGet(string id)
        {
            var post = await _api.Posts.GetByIdAsync<Aero.Cms.Models.PostInfo>(id);

            if (post == null)
            {
                return NotFound();
            }

            Id = post.Id;
            Permalink = post.Permalink + "?draft=true";

            return Page();
        }
    }
}