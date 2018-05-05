/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Piranha.AspNetCore.Identity.Models
{
    public class UserEditModel
    {
        public Data.User User { get; set; }
        public IList<Data.Role> Roles { get; set; }
        public IList<string> SelectedRoles { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }

        public UserEditModel() 
        {
            Roles = new List<Data.Role>();
            SelectedRoles = new List<string>();
        }

        public static UserEditModel Create(Db db) 
        {
            return new UserEditModel
            {
                User = new Data.User(),
                Roles = db.Roles.OrderBy(r => r.Name).ToList()
            };
        }

        public static UserEditModel GetById(Db db, Guid id) 
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

        public async Task<bool> Save(Db db, UserManager<Data.User> userManager)
        {
            var user = db.Users.FirstOrDefault(u => u.Id == User.Id);

            if (user == null)
            {
                user = new Data.User {
                    Id = User.Id != Guid.Empty ? User.Id : Guid.NewGuid(),
                    UserName = User.UserName,
                    Email = User.Email
                };
                User.Id = user.Id;

                var createResult = await userManager.CreateAsync(user, "password");
            }
            else 
            {
                user.UserName = User.UserName;
                user.Email = User.Email;
            }

            // Remove current roles.
            foreach (var userRole in db.UserRoles.Where(r => r.UserId == user.Id))
            {
                var role = db.Roles.FirstOrDefault(r => r.Id == userRole.RoleId);

                if (role != null)
                    await userManager.RemoveFromRoleAsync(user, role.Name);
            }
            
            // Add the currently selected roles.
            foreach (var role in SelectedRoles)
            {
                await userManager.AddToRoleAsync(user, role);
            }
            db.SaveChanges();

            return true;
        }
    }
}