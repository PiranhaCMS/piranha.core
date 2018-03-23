/*
 * Copyright (c) 2018 Håkan Edling
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

namespace Piranha.AspNetCore.Identity.Models
{
    public class UserEditModel
    {
        public Data.User User { get; set; }
        public IList<Data.Role> Roles { get; set; }
        public IList<string> SelectedRoles { get; set; }

        public UserEditModel() 
        {
            Roles = new List<Data.Role>();
            SelectedRoles = new List<string>();
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

        public bool Save(Db db)
        {
            var user = db.Users.FirstOrDefault(u => u.Id == User.Id);

            if (user == null)
            {
                User.Id = User.Id != Guid.Empty ? User.Id : Guid.NewGuid();                
            }
            user.UserName = User.UserName;
            user.Email = User.Email;

            db.SaveChanges();

            return true;
        }
    }
}