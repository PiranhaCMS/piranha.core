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
using Piranha.Manager.Models;
using Piranha.Manager.Services;

namespace Piranha.Manager.Controllers;

/// <summary>
/// Api controller for module management.
/// </summary>
[Area("Manager")]
[Route("manager/api/module")]
[Authorize(Policy = Permission.Admin)]
[ApiController]
[AutoValidateAntiforgeryToken]
public class ModuleApiController : Controller
{
    private readonly ModuleService _service;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public ModuleApiController(ModuleService service)
    {
        _service = service;
    }

    /// <summary>
    /// Gets the list model.
    /// </summary>
    /// <returns>The list model</returns>
    [Route("list")]
    [HttpGet]
    [Authorize(Policy = Permission.Modules)]
    public ModuleListModel List()
    {
        return _service.GetList();
    }
}
