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
[Route("api/media")]
public class MediaApiController : Controller
{
    private readonly IApi _api;
    private readonly IAuthorizationService _auth;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="api">The current api</param>
    /// <param name="auth">The authorization service</param>
    public MediaApiController(IApi api, IAuthorizationService auth)
    {
        _api = api;
        _auth = auth;
    }

    /// <summary>
    /// Gets the media asset with the specified id.
    /// </summary>
    /// <param name="id">The media id</param>
    /// <returns>The media asset</returns>
    [HttpGet]
    [Route("{id:Guid}")]
    public virtual async Task<IActionResult> GetById(Guid id)
    {
        if (!Module.AllowAnonymousAccess)
        {
            if (!(await _auth.AuthorizeAsync(User, Permissions.Media)).Succeeded)
            {
                return Unauthorized();
            }
        }
        return Json(await _api.Media.GetByIdAsync(id));
    }

    /// <summary>
    /// Gets all of the media assets located in the folder
    /// with the specified id. Not providing a folder id will
    /// return all of the media assets at root level.
    /// </summary>
    /// <param name="folderId">The optional folder id</param>
    /// <returns></returns>
    [HttpGet]
    [Route("list/{folderId:Guid?}")]
    public virtual async Task<IActionResult> GetByFolderId(Guid? folderId = null)
    {
        if (!Module.AllowAnonymousAccess)
        {
            if (!(await _auth.AuthorizeAsync(User, Permissions.Media)).Succeeded)
            {
                return Unauthorized();
            }
        }
        return Json(await _api.Media.GetAllByFolderIdAsync(folderId));
    }

    /// <summary>
    /// Gets the media folder structure.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("structure")]
    public virtual async Task<IActionResult> GetStructure()
    {
        if (!Module.AllowAnonymousAccess)
        {
            if (!(await _auth.AuthorizeAsync(User, Permissions.Media)).Succeeded)
            {
                return Unauthorized();
            }
        }
        return Json(await _api.Media.GetStructureAsync());
    }
}
