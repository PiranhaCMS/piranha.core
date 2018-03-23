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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Piranha.AspNetCore.Identity.Models
{
    public class RoleEditModel
    {
        public Data.Role Role { get; set; }
        public IList<string> SelectedClaims { get; set; }

        public RoleEditModel() 
        {
            SelectedClaims = new List<string>();
        }

        public static RoleEditModel GetById(Db db, Guid id) 
        {
            var role = db.Roles.FirstOrDefault(r => r.Id == id);

            if (role != null)
            {
                var model = new RoleEditModel
                {
                    Role = role
                };

                var roleClaims = db.RoleClaims.Where(r => r.RoleId == id).ToList();
                foreach (var claim in roleClaims)
                {
                    model.SelectedClaims.Add(claim.ClaimType);
                }
                return model;
            }
            return null;
        }

        public static RoleEditModel Create()
        {
            return new RoleEditModel
            {
                Role = new Data.Role()
            };
        }

        public bool Save(Db db) 
        {
            var role = db.Roles.FirstOrDefault(r => r.Id == Role.Id);

            if (role == null)
            {
                Role.Id = Role.Id != Guid.Empty ? Role.Id : Guid.NewGuid();
                Role.NormalizedName = !string.IsNullOrEmpty(Role.NormalizedName) ? Role.NormalizedName.ToUpper() : Role.Name.ToUpper();
                
                role = new Data.Role {
                    Id = Role.Id
                };
                db.Roles.Add(role);
            }
            role.Name = Role.Name;
            role.NormalizedName = Role.NormalizedName;

            var claims = db.RoleClaims.Where(r => r.RoleId == role.Id).ToList();
            var delete = new List<IdentityRoleClaim<Guid>>();
            var add = new List<IdentityRoleClaim<Guid>>();

            foreach (var old in claims)
            {
                if (!SelectedClaims.Contains(old.ClaimType))
                    delete.Add(old);
            }

            foreach (var selected in SelectedClaims)
            {
                if (!claims.Any(c => c.ClaimType == selected))
                    add.Add(new IdentityRoleClaim<Guid>()
                    {
                        RoleId = role.Id,
                        ClaimType = selected,
                        ClaimValue = selected
                    });
            }

            db.RoleClaims.RemoveRange(delete);
            db.RoleClaims.AddRange(add);

            db.SaveChanges();

            return true;
        }
    }
}