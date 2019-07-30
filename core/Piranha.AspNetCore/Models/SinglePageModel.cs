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
using Microsoft.AspNetCore.Mvc;
using Piranha.AspNetCore.Services;
using Piranha.Models;

namespace Piranha.AspNetCore.Models
{
    public class SinglePageModel<T> : Microsoft.AspNetCore.Mvc.RazorPages.PageModel where T : PageBase
    {
        protected readonly IApi _api;
        protected readonly IModelLoader _loader;

        /// <summary>
        /// Gets/sets the model data.
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public SinglePageModel(IApi api, IModelLoader loader)
        {
            _api = api;
            _loader = loader;
        }

        /// <summary>
        /// Gets the model data.
        /// </summary>
        /// <param name="id">The requested model id</param>
        /// <param name="draft">If the draft should be fetched</param>
        public virtual async Task<IActionResult> OnGet(Guid id, bool draft = false)
        {
            Data = await _loader.GetPage<T>(id, HttpContext.User, draft);

            if (Data == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}