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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Piranha.AspNetCore.Identity.Data;
using Piranha.AspNetCore.Identity.Models;
using Piranha.Manager;
using Piranha.Manager.Controllers;
using Piranha.Manager.Models;

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
        private readonly ManagerLocalizer _localizer;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        /// <param name="userManager">The current user manager</param>
        /// <param name="localizer">The manager localizer</param>
        public UserController(IDb db, UserManager<User> userManager, ManagerLocalizer localizer)
        {
            _db = db;
            _userManager = userManager;
            _localizer = localizer;
        }

        /// <summary>
        /// Gets the list view with the currently available users.
        /// </summary>
        [HttpGet]
        [Route("/manager/users")]
        [Authorize(Policy = Permissions.Users)]
        public IActionResult List()
        {
            return View();
        }

        /// <summary>
        /// Gets the list view with the currently available users.
        /// </summary>
        [HttpGet]
        [Route("/manager/users/list")]
        [Authorize(Policy = Permissions.Users)]
        public UserListModel Get()
        {
            return UserListModel.Get(_db);
        }

        /// <summary>
        /// Gets the edit view for an existing user.
        /// </summary>
        /// <param name="id">The user id</param>
        [HttpGet]
        [Route("/manager/user/{id:Guid?}")]
        [Authorize(Policy = Permissions.UsersEdit)]
        public IActionResult Edit(Guid id)
        {
            return View(id);
            //return View(UserEditModel.GetById(_db, id));
        }

        /// <summary>
        /// Gets the edit view for an existing user.
        /// </summary>
        /// <param name="id">The user id</param>
        [HttpGet]
        [Route("/manager/user/edit/{id:Guid}")]
        [Authorize(Policy = Permissions.UsersEdit)]
        public UserEditModel Get(Guid id)
        {
            return UserEditModel.GetById(_db, id);
            //return View(UserEditModel.GetById(_db, id));
        }

        /// <summary>
        /// Gets the edit view for a new user.
        /// </summary>
        [HttpGet]
        [Route("/manager/user/add")]
        [Authorize(Policy = Permissions.UsersEdit)]
        public UserEditModel Add()
        {
            return UserEditModel.Create(_db);
            //return View("Edit", UserEditModel.Create(_db));
        }

        /// <summary>
        /// Saves the given user.
        /// </summary>
        /// <param name="model">The user model</param>
        [HttpPost]
        [Route("/manager/user/save")]
        [Authorize(Policy = Permissions.UsersSave)]
        public async Task<IActionResult> Save([FromBody] UserEditModel model)
        {
            // Refresh roles in the model if validation fails
            //var temp = UserEditModel.Create(_db);
            //model.Roles = temp.Roles;

            if(model.User == null)
            {
                return BadRequest(GetErrorMessage(_localizer.Security["The user could not be found."]));
            }

            try
            {
                var userId = model.User.Id;
                var isNew = userId == Guid.Empty;

                if (string.IsNullOrWhiteSpace(model.User.UserName))
                {
                    return BadRequest(GetErrorMessage(_localizer.General["Username is mandatory."]));
                }

                if (string.IsNullOrWhiteSpace(model.User.Email))
                {
                    return BadRequest(GetErrorMessage(_localizer.General["Email address is mandatory."]));
                }

                if (!string.IsNullOrWhiteSpace(model.Password) && model.Password != model.PasswordConfirm)
                {
                    return BadRequest(GetErrorMessage(string.Format("{0} {1} - {2}", _localizer.Security["The new passwords does not match."], model.Password, model.PasswordConfirm)));
                }

                if (model.User.Id == Guid.Empty && string.IsNullOrWhiteSpace(model.Password))
                {
                    return BadRequest(GetErrorMessage(_localizer.Security["Password is mandatory when creating a new user."]));
                }

                if (!string.IsNullOrWhiteSpace(model.Password) && _userManager.PasswordValidators.Count > 0)
                {
                    var errors = new List<string>();
                    foreach (var validator in _userManager.PasswordValidators)
                    {
                        var errorResult = await validator.ValidateAsync(_userManager, model.User, model.Password);
                        if (!errorResult.Succeeded)
                            errors.AddRange(errorResult.Errors.Select(msg => msg.Description));
                        if (errors.Count > 0)
                        {
                            return BadRequest(GetErrorMessage(string.Join("<br />", errors)));
                        }
                    }
                }

                //check username
                if (await _db.Users.CountAsync(u => u.UserName.ToLower().Trim() == model.User.UserName.ToLower().Trim() && u.Id != userId) > 0)
                {
                    return BadRequest(GetErrorMessage(_localizer.Security["Username is used by another user."]));
                }

                //check email
                if (await _db.Users.CountAsync(u => u.Email.ToLower().Trim() == model.User.Email.ToLower().Trim() && u.Id != userId) > 0)
                {
                    return BadRequest(GetErrorMessage(_localizer.Security["Email address is used by another user."]));
                }

                var result = await model.Save(_userManager);
                if (result.Succeeded)
                {
                    return Ok(Get(model.User.Id));
                }

                var errorMessages = new List<string>();
                errorMessages.AddRange(result.Errors.Select(msg => msg.Description));

                return BadRequest(GetErrorMessage(_localizer.Security["The user could not be saved."] + "<br/><br/>" + string.Join("<br />", errorMessages)));
            }
            catch (Exception ex)
            {
                return BadRequest(GetErrorMessage(ex.Message));
            }
        }

        /// <summary>
        /// Deletes the user with the given id.
        /// </summary>
        /// <param name="id">The user id</param>
        [HttpGet]
        [Route("/manager/user/delete/{id:Guid}")]
        [Authorize(Policy = Permissions.UsersSave)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == id);

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser != null && user.Id == currentUser.Id)
            {
                return BadRequest(GetErrorMessage(_localizer.Security["Can't delete yourself."]));
            }

            if (user != null)
            {
                _db.Users.Remove(user);
                _db.SaveChanges();

                return Ok(GetSuccessMessage(_localizer.Security["The user has been deleted."]));
            }

            return NotFound(GetErrorMessage(_localizer.Security["The user could not be found."]));
        }

        private AliasListModel GetSuccessMessage(string message)
        {
            return GetMessage(message, StatusMessage.Success);
        }

        private AliasListModel GetErrorMessage(string errorMessage)
        {
            return GetMessage(!string.IsNullOrWhiteSpace(errorMessage) ? errorMessage :  _localizer.General["An error occurred"], StatusMessage.Error);
        }

        private AliasListModel GetMessage(string message, string type)
        {
            var result = new AliasListModel();
            result.Status = new StatusMessage
            {
                Type = type,
                Body = message
            };
            return result;
        }
    }
}