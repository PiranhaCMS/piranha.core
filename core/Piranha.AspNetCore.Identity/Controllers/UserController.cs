/*
 * Copyright (c) 2018-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Piranha.AspNetCore.Identity.Data;
using Piranha.AspNetCore.Identity.Models;
using Piranha.Manager.Controllers;

namespace Piranha.AspNetCore.Identity.Controllers
{
    /// <summary>
    /// Manager controller for managing users accounts.
    /// </summary>
    [Area("Manager")]
    public class UserController : ManagerController
    {
        private readonly IDb _db;
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        /// <param name="userManager">The current user manager</param>
        public UserController(IDb db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        /// <summary>
        /// Gets the list view with the currently available users.
        /// </summary>
        [Route("/manager/users")]
        [Authorize(Policy = Permissions.Users)]
        public IActionResult List()
        {
            return View(UserListModel.Get(_db));
        }

        /// <summary>
        /// Gets the edit view for an existing user.
        /// </summary>
        /// <param name="id">The user id</param>
        [Route("/manager/user/{id:Guid}")]
        [Authorize(Policy = Permissions.UsersEdit)]
        public IActionResult Edit(Guid id)
        {
            return View(UserEditModel.GetById(_db, id));
        }

        /// <summary>
        /// Gets the edit view for a new user.
        /// </summary>
        [Route("/manager/user/add")]
        [Authorize(Policy = Permissions.UsersEdit)]
        public IActionResult Add()
        {
            return View("Edit", UserEditModel.Create(_db));
        }

        /// <summary>
        /// Saves the given user.
        /// </summary>
        /// <param name="model">The user model</param>
        [HttpPost]
        [Route("/manager/user/save")]
        [Authorize(Policy = Permissions.UsersSave)]
        public async Task<IActionResult> Save(UserEditModel model)
        {
            // Refresh roles in the model if validation fails
            var temp = UserEditModel.Create(_db);
            model.Roles = temp.Roles;

            if (string.IsNullOrWhiteSpace(model.User.UserName))
            {
                ErrorMessage("User name is mandatory.", false);
                return View("Edit", model);
            }

            if (string.IsNullOrWhiteSpace(model.User.Email))
            {
                ErrorMessage("Email is mandatory.", false);
                return View("Edit", model);
            }

            if (!string.IsNullOrWhiteSpace(model.Password) && model.Password != model.PasswordConfirm)
            {
                ErrorMessage($"The new passwords does not match. {model.Password} - {model.PasswordConfirm}", false);
                return View("Edit", model);
            }

            if (model.User.Id == Guid.Empty && string.IsNullOrWhiteSpace(model.Password))
            {
                ErrorMessage("Password is mandatory when creating a new user.", false);
                return View("Edit", model);
            }

            if (!string.IsNullOrWhiteSpace(model.Password) && _userManager.PasswordValidators.Count > 0)
            {
                var errors = new List<string>();
                foreach (var validator in _userManager.PasswordValidators)
                {
                    var result = await validator.ValidateAsync(_userManager, model.User, model.Password);
                    if (!result.Succeeded)
                        errors.AddRange(result.Errors.Select(msg => msg.Description));
                    if (errors.Count > 0)
                    {
                        ErrorMessage(string.Join("<br />", errors), false);
                        return View("Edit", model);
                    }
                }
            }

            if (await model.Save(_userManager))
            {
                SuccessMessage("The user has been saved.");
                return RedirectToAction("Edit", new {id = model.User.Id});
            }

            ErrorMessage("The user could not be saved.", false);
            return View("Edit", model);
        }

        /// <summary>
        /// Deletes the user with the given id.
        /// </summary>
        /// <param name="id">The user id</param>
        [Route("/manager/user/delete/{id:Guid}")]
        [Authorize(Policy = Permissions.UsersSave)]
        public IActionResult Delete(Guid id)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == id);

            if (user != null)
            {
                _db.Users.Remove(user);
                _db.SaveChanges();

                SuccessMessage("The user has been deleted.");
                return RedirectToAction("List");
            }

            ErrorMessage("Could not find the user to delete.");
            return RedirectToAction("List");
        }
    }
}