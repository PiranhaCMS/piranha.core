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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ManagerLocalizer _localizer;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="userManager">The current user manager</param>
        /// <param name="roleManager">The current role manager</param>
        /// <param name="localizer">The manager localizer</param>
        public UserController(UserManager<User> userManager, RoleManager<Role> roleManager, ManagerLocalizer localizer)
        {
            _localizer = localizer;
            _userManager = userManager;
            _roleManager = roleManager;
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
        public async Task<UserListModel> Get()
        {
            var userListModel = new UserListModel();

            foreach (var u in await GetUsers())
            {
                userListModel.Users.Add(new UserListModel.ListItem()
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    GravatarUrl = !string.IsNullOrWhiteSpace(u.Email) ? Utils.GetGravatarUrl(u.Email, 25) : null,
                    Roles = await _userManager.GetRolesAsync(u)
                });
            }

            return userListModel;

            async Task<List<User>> GetUsers()
            {
                if (_userManager.SupportsQueryableUsers)
                {
                    return _userManager.Users.ToList();
                }
                if (_userManager.SupportsUserRole)
                {
                    return (await _userManager.GetUsersInRoleAsync(default)).ToList();
                }

                throw new NotSupportedException($"");
            }
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
        public async Task<UserEditModel> Get(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return null;
            }
            return new UserEditModel
            {
                User = user,
                SelectedRoles = await _userManager.GetRolesAsync(user),
                Roles = _roleManager.Roles.OrderBy(r => r.Name).ToList()
            };
        }

        /// <summary>
        /// Gets the edit view for a new user.
        /// </summary>
        [HttpGet]
        [Route("/manager/user/add")]
        [Authorize(Policy = Permissions.UsersEdit)]
        public UserEditModel Add()
        {
            return new()
            {
                User = new User(),
                Roles = _roleManager.Roles.OrderBy(r => r.Name).ToList()
            };
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

            if (model.User == null)
            {
                return BadRequest(GetErrorMessage(_localizer.Security["The user could not be found."]));
            }

            try
            {
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
                if (await _userManager.FindByNameAsync(model.User.UserName.ToLower().Trim()) is not null)
                {
                    return BadRequest(GetErrorMessage(_localizer.Security["Username is used by another user."]));
                }

                //check email
                if (await _userManager.FindByEmailAsync(model.User.Email.ToLower().Trim()) is not null)
                {
                    return BadRequest(GetErrorMessage(_localizer.Security["Email address is used by another user."]));
                }

                var result = await _userManager.UpdateAsync(model.User);
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
        [Authorize(Policy = Permissions.UsersDelete)]
        public async Task<IActionResult> Delete(Guid id)
        {

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser?.Id == id)
            {
                return BadRequest(GetErrorMessage(_localizer.Security["Can't delete yourself."]));
            }

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
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