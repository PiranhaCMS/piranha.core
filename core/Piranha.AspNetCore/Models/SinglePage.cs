/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Mvc;
using Piranha.AspNetCore.Services;
using Piranha.Models;

namespace Piranha.AspNetCore.Models;

/// <summary>
/// Razor Page model for a single page.
/// </summary>
/// <typeparam name="T">The page type</typeparam>
public class SinglePage<T> : Microsoft.AspNetCore.Mvc.RazorPages.PageModel where T : PageBase
{
    /// <summary>
    /// The current api.
    /// </summary>
    protected readonly IApi _api;

    /// <summary>
    /// The current model loader.
    /// </summary>
    protected readonly IModelLoader _loader;

    /// <summary>
    /// Gets/sets the model data.
    /// </summary>
    public T Data { get; set; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="api">The current api</param>
    /// <param name="loader">The model loader</param>
    public SinglePage(IApi api, IModelLoader loader)
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
        try
        {
            Data = await _loader.GetPageAsync<T>(id, HttpContext.User, draft);

            if (Data == null)
            {
                return NotFound();
            }
            return Page();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }
}
