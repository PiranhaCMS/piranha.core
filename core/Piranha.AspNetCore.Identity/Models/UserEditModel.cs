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
using Microsoft.AspNetCore.Identity;
using Piranha.AspNetCore.Identity.Data;

namespace Piranha.AspNetCore.Identity.Models
{
    public class UserEditModel
    {
        public User User { get; set; }
        public IList<Role> Roles { get; set; } = new List<Role>();
        public IList<string> SelectedRoles { get; set; } = new List<string>();
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }

        public static UserEditModel Create(IDb db)
        {
            return new UserEditModel
            {
                User = new User(),
                Roles = db.Roles.OrderBy(r => r.Name).ToList()
            };
        }

        public static UserEditModel GetById(IDb db, Guid id)
        {
            var user = db.Users.FirstOrDefault(u => u.Id == id);

            if (user != null)
            {
                var model = new UserEditModel
                {
                    User = user,
                    Roles = db.Roles.OrderBy(r => r.Name).ToList()
                };

                var userRoles = db.UserRoles.Where(r => r.UserId == id).ToList();
                foreach (var role in userRoles)
                {
                    model.SelectedRoles.Add(model.Roles.Single(r => r.Id == role.RoleId).Name);
                }
                return model;
            }

            return null;
        }

        public async Task<IdentityResult> Save(UserManager<User> userManager)
        {
            IdentityResult result;
            var user = await userManager.FindByIdAsync(User.Id.ToString());

            if (user == null)
            {
                user = new User
                {
                    Id = User.Id != Guid.Empty ? User.Id : Guid.NewGuid(),
                    UserName = User.UserName,
                    Email = User.Email
                };
                User.Id = user.Id;

                result = await userManager.CreateAsync(user, Password);
                if (!result.Succeeded) {
                    return result;
                }
            }
            else
            {
                result = await userManager.SetUserNameAsync(user, User.UserName);
                if (!result.Succeeded)
                {
                    return result;
                }

                result = await userManager.SetEmailAsync(user, User.Email);
                if (!result.Succeeded)
                {
                    return result;
                }
            }

            // Remove old roles
            var roles = await userManager.GetRolesAsync(user);
            result = await userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                return result;
            }

            // Add current roles
            result = await userManager.AddToRolesAsync(user, SelectedRoles);
            if (!result.Succeeded)
            {
                return result;
            }

            if (!string.IsNullOrWhiteSpace(Password))
            {
                result = await userManager.RemovePasswordAsync(user);
                if (!result.Succeeded)
                {
                    return result;
                }
                result = await userManager.AddPasswordAsync(user, Password);
                if (!result.Succeeded)
                {
                    return result;
                }
            }

            return result;
        }
    }
}