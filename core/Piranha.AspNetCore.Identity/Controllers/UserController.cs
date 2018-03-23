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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Piranha.AspNetCore.Identity.Controllers
{
    [Area("Manager")]
    public class UserController : Areas.Manager.Controllers.MessageControllerBase
    {
        private readonly Db _db;
        private UserManager<Data.User> _userManager;

        public UserController(Db db, UserManager<Data.User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [Route("/manager/users")]
        [Authorize(Policy = Permissions.Users)]
        public IActionResult List()
        {
            return View(Models.UserListModel.Get(_db));
        }

        [Route("/manager/user/{id:Guid}")]
        [Authorize(Policy = Permissions.UsersEdit)]
        public IActionResult Edit(Guid id)
        {
            return View(Models.UserEditModel.GetById(_db, id));
        }

        [HttpPost]
        [Route("/manager/user/save")]
        [Authorize(Policy = Permissions.UsersSave)]
        public IActionResult Save(Models.UserEditModel model)
        {
            if (model.Save(_db))
            {
                SuccessMessage("The user has been saved.");
                return RedirectToAction("Edit", new { id = model.User.Id });
            }
            ErrorMessage("The user could not be saved.", false);
            return View("Edit", model);        
        }
    }
}