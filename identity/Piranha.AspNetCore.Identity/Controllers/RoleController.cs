/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.AspNetCore.Identity.Models;
using Piranha.Manager.Controllers;

namespace Piranha.AspNetCore.Identity.Controllers
{
    [Area("Manager")]
    public class RoleController : ManagerController
    {
        private readonly IDb _db;

        public RoleController(IDb db)
        {
            _db = db;
        }
        
        [HttpGet]
        [Route("/manager/roles")]
        [Authorize(Policy = Permissions.Roles)]
        public IActionResult List()
        {
            return View(RoleListModel.Get(_db));
        }

        [HttpGet]
        [Route("/manager/role/{id:Guid}")]
        [Authorize(Policy = Permissions.RolesEdit)]
        public IActionResult Edit(Guid id)
        {
            return View("Edit", RoleEditModel.GetById(_db, id));
        }

        [HttpGet]
        [Route("/manager/role")]
        [Authorize(Policy = Permissions.RolesAdd)]
        public IActionResult Add()
        {
            return View("Edit", RoleEditModel.Create());
        }

        [HttpPost]
        [Route("/manager/role/save")]
        [Authorize(Policy = Permissions.RolesSave)]
        public IActionResult Save(RoleEditModel model)
        {
            if (model.Save(_db))
            {
                SuccessMessage("The role has been saved.");
                return RedirectToAction("Edit", new {id = model.Role.Id});
            }

            ErrorMessage("The role could not be saved.", false);
            return View("Edit", model);
        }

        [HttpGet]
        [Route("/manager/role/delete")]
        [Authorize(Policy = Permissions.RolesDelete)]
        public IActionResult Delete(Guid id)
        {
            var role = _db.Roles
                .FirstOrDefault(r => r.Id == id);

            if (role != null)
            {
                _db.Roles.Remove(role);
                _db.SaveChanges();

                SuccessMessage("The role has been deleted.");
                return RedirectToAction("List");
            }

            ErrorMessage("The role could not be deleted.", false);
            return RedirectToAction("List");
        }
    }
}