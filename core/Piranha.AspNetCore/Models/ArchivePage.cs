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
/// Razor Page model for an archive page.
/// </summary>
/// <typeparam name="T">The page type</typeparam>
public class ArchivePage<T> : ArchivePage<T, PostInfo>
    where T : PageBase
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="api">The current api</param>
    /// <param name="loader">The model loader</param>
    public ArchivePage(IApi api, IModelLoader loader) : base(api, loader) { }
}

/// <summary>
/// Razor Page model for an archive page.
/// </summary>
/// <typeparam name="T">The page type</typeparam>
/// <typeparam name="TPost">The post type</typeparam>
public class ArchivePage<T, TPost> : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    where T : PageBase
    where TPost: PostBase
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
    /// Gets/sets the post archive model.
    /// </summary>
    public PostArchive<TPost> Archive { get; set; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="api">The current api</param>
    /// <param name="loader">The model loader</param>
    public ArchivePage(IApi api, IModelLoader loader)
    {
        _api = api;
        _loader = loader;
    }

    /// <summary>
    /// Gets the model data.
    /// </summary>
    /// <param name="id">The requested model id</param>
    /// <param name="year">The optionally requested year</param>
    /// <param name="month">The optionally requested month</param>
    /// <param name="pagenum">The optionally requested page</param>
    /// <param name="category">The optionally requested category</param>
    /// <param name="tag">The optionally requested tag</param>
    /// <param name="draft">If the draft should be fetched</param>
    public virtual async Task<IActionResult> OnGet(Guid id, int? year = null, int? month = null,
        int? pagenum = null, Guid? category = null, Guid? tag = null, bool draft = false)
    {
        try
        {
            Data = await _loader.GetPageAsync<T>(id, HttpContext.User, draft);

            if (Data == null)
            {
                return NotFound();
            }

            Archive = await _api.Archives.GetByIdAsync<TPost>(id, pagenum, category, tag, year, month);

            return Page();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }
}
