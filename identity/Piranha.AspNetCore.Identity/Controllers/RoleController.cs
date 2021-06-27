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
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Piranha.AspNetCore.Identity.Data;
using Piranha.AspNetCore.Identity.Models;
using Piranha.Manager.Controllers;

namespace Piranha.AspNetCore.Identity.Controllers
{
    [Area("Manager")]
    public class RoleController : ManagerController
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="userManager">The current user manager</param>
        /// <param name="roleManager">The current role manager</param>
        public RoleController(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        [Route("/manager/roles")]
        [Authorize(Policy = Permissions.Roles)]
        public async Task<IActionResult> List()
        {
            var model = new RoleListModel
            {
                Roles = _roleManager.Roles
                    .OrderBy(r => r.Name)
                    .Select(r => new RoleListModel.ListItem
                    {
                        Id = r.Id,
                        Name = r.Name
                    }).ToList()
            };

            foreach (var role in model.Roles)
            {
                role.UserCount = (await _userManager.GetUsersInRoleAsync(role.Id.ToString())).Count;
            }

            return View(model);
        }

        [HttpGet]
        [Route("/manager/role/{id:Guid}")]
        [Authorize(Policy = Permissions.RolesEdit)]
        public async Task<IActionResult> Edit(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());

            if (role == null)
            {
                return View("Edit", null);
            }

            var model = new RoleEditModel
            {
                Role = role,
                SelectedClaims = (await _roleManager.GetClaimsAsync(role)).Select(claim => claim.Type).ToList()
            };
            return View("Edit", model);
        }

        [HttpGet]
        [Route("/manager/role")]
        [Authorize(Policy = Permissions.RolesAdd)]
        public IActionResult Add()
        {
            return View("Edit", new RoleEditModel() {Role = new Role()});
        }

        [HttpPost]
        [Route("/manager/role/save")]
        [Authorize(Policy = Permissions.RolesSave)]
        public async Task<IActionResult> Save(RoleEditModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Role.Id.ToString());

            if (role == null)
            {
                await _roleManager.CreateAsync(model.Role);
            }
            else
            {
                role.Name = model.Role.Name;

                var currentClaims = await _roleManager.GetClaimsAsync(role);

                foreach (var old in currentClaims)
                {
                    if (model.SelectedClaims.Contains(old.Type) == false)
                    {
                        await _roleManager.RemoveClaimAsync(role, old);
                    }
                }

                foreach (var selected in model.SelectedClaims)
                {
                    if (currentClaims.All(c => c.Type != selected))
                    {
                        await _roleManager.AddClaimAsync(role, new Claim(selected, selected));
                    }
                }

                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    SuccessMessage("The role has been saved.");
                    return RedirectToAction("Edit", new {id = model.Role.Id});
                }
            }

            ErrorMessage("The role could not be saved.", false);
            return View("Edit", model);
        }

        [HttpGet]
        [Route("/manager/role/delete")]
        [Authorize(Policy = Permissions.RolesDelete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                SuccessMessage("The role has been deleted.");
            }
            else
            {
                ErrorMessage("The role could not be deleted.", false);
            }

            return RedirectToAction("List");
        }
    }
}
