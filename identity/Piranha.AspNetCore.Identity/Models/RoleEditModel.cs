/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Identity;
using Piranha.AspNetCore.Identity.Data;
using Raven.Client.Documents.Linq;

namespace Piranha.AspNetCore.Identity.Models;

public class RoleEditModel
{
    public RoleEditModel()
    {
        SelectedClaims = new List<string>();
    }

    public Role Role { get; set; }
    public List<string> SelectedClaims { get; set; }

    public static RoleEditModel GetById(IIdentityDb db, string id)
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
            Role = new Role()
        };
    }

    public bool Save(IIdentityDb db)
    {
        var role = db.Roles.FirstOrDefault(r => r.Id == Role.Id);

        if (role == null)
        {
            Role.Id = Role.Id != string.Empty ? Role.Id : Snowflake.NewId();
            Role.NormalizedName = !string.IsNullOrEmpty(Role.NormalizedName)
                ? Role.NormalizedName.ToUpper()
                : Role.Name.ToUpper();

            role = new Role
            {
                Id = Role.Id
            };
            //db.Roles.Add(role);
            db.session.StoreAsync(role);
        }

        role.Name = Role.Name;
        role.NormalizedName = Role.NormalizedName;

        var claims = db.RoleClaims.Where(r => r.RoleId == role.Id).ToList();
        var delete = new List<IdentityRoleClaim<string>>();
        var add = new List<IdentityRoleClaim<string>>();

        foreach (var old in claims)
        {
            if (!SelectedClaims.Contains(old.ClaimType))
            {
                delete.Add(old);
            }
        }

        foreach (var selected in SelectedClaims)
        {
            if (!claims.Any(c => c.ClaimType == selected))
            {
                add.Add(new IdentityRoleClaim<string>
                {
                    RoleId = role.Id,
                    ClaimType = selected,
                    ClaimValue = selected
                });
            }
        }

        // db.RoleClaims.RemoveRange(delete);
        // db.RoleClaims.AddRange(add);
        foreach (var r in delete)
        {
            db.session.Delete(r);
        }
        
        foreach (var r in add)        {
            db.session.StoreAsync(r);
        }

        db.SaveChanges();

        return true;
    }
}
