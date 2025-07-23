/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Piranha.WebApi;

[ApiController]
[Route("api/sitemap")]
public class SitemapApiController : Controller
{
    private readonly IApi _api;
    private readonly IAuthorizationService _auth;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="api">The current api</param>
    /// <param name="auth">The authorization service</param>
    public SitemapApiController(IApi api, IAuthorizationService auth)
    {
        _api = api;
        _auth = auth;
    }

    /// <summary>
    /// Gets the public sitemap for the default or specified site.
    /// </summary>
    /// <param name="id">The optional site id</param>
    /// <returns>The sitemap</returns>
    [HttpGet]
    [Route("{id:Guid?}")]
    public virtual async Task<IActionResult> GetById(Guid? id = null)
    {
        if (!Module.AllowAnonymousAccess)
        {
            if (!(await _auth.AuthorizeAsync(User, Permissions.Sitemap)).Succeeded)
            {
                return Unauthorized();
            }
        }
        return Json(await _api.Sites.GetSitemapAsync(id));
    }
}
