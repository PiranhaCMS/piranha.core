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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Models;

namespace Piranha.AspNetCore.Models
{
    public class SinglePageModel<T> : Microsoft.AspNetCore.Mvc.RazorPages.PageModel where T : PageBase
    {
        protected readonly IApi _api;
        protected readonly IAuthorizationService _auth;

        /// <summary>
        /// Gets/sets the model data.
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public SinglePageModel(IApi api, IAuthorizationService auth)
        {
            _api = api;
            _auth = auth;
        }

        /// <summary>
        /// Gets the model data.
        /// </summary>
        /// <param name="id">The requested model id</param>
        /// <param name="draft">If the draft should be fetched</param>
        public virtual async Task<IActionResult> OnGet(Guid id, bool draft = false)
        {
            // Check if we're requesting a draft
            if (draft)
            {
                // Check that the current user is authorized to preview pages
                if ((await _auth.AuthorizeAsync(HttpContext.User, Security.Permission.PagePreview)).Succeeded)
                {
                    // Get the draft, if available
                    Data = await _api.Pages.GetDraftByIdAsync<T>(id);

                    if (Data == null)
                    {
                        Data = await _api.Pages.GetByIdAsync<T>(id);
                    }
                }
            }

            // No draft loaded or requested, try to get the published page
            if (Data == null)
            {
                Data = await _api.Pages.GetByIdAsync<T>(id);

                if (Data != null)
                {
                    // Make sure the page is published
                    if (!Data.Published.HasValue || Data.Published.Value > DateTime.Now)
                    {
                        // No published version exists
                        return NotFound();
                    }
                }
                else
                {
                    // No page found with the specified id
                    return NotFound();
                }
            }
            return Page();
        }
    }
}