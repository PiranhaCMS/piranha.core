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
    [Authorize(Policy = Security.Permission.PostPreview)]
    public class PostPreviewModel : PageModel
    {
        private readonly IApi _api;
        public Guid Id { get; set; }
        public string Permalink { get; set; }

        public PostPreviewModel(IApi api)
        {
            _api = api;
        }

        public async Task<IActionResult> OnGet(Guid id)
        {
            var post = await _api.Posts.GetByIdAsync<Piranha.Models.PostInfo>(id);

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