/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Piranha.AspNetCore.Identity.Controllers
{
    [Area("Manager")]
    public class RoleController : Areas.Manager.Controllers.MessageControllerBase
    {
        private readonly IDb _db;

        public RoleController(IDb db)
        {
            _db = db;
        }

        [Route("/manager/roles")]
        [Authorize(Policy = Permissions.Roles)]
        public IActionResult List()
        {
            return View(Models.RoleListModel.Get(_db));
        }

        [Route("/manager/role/{id:Guid}")]
        [Authorize(Policy = Permissions.RolesEdit)]
        public IActionResult Edit(Guid id)
        {
            return View("Edit", Models.RoleEditModel.GetById(_db, id));
        }

        [Route("/manager/role")]
        [Authorize(Policy = Permissions.RolesAdd)]
        public IActionResult Add()
        {
            return View("Edit", Models.RoleEditModel.Create());
        }

        [HttpPost]
        [Route("/manager/role/save")]
        [Authorize(Policy = Permissions.RolesSave)]
        public IActionResult Save(Models.RoleEditModel model)
        {
            if (model.Save(_db))
            {
                SuccessMessage("The role has been saved.");
                return RedirectToAction("Edit", new { id = model.Role.Id });
            }
            ErrorMessage("The role could not be saved.", false);
            return View("Edit", model);
        }
    }
}