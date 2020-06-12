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

namespace Piranha.AspNetCore.Identity.Models
{
    public class RoleListModel
    {
        public RoleListModel()
        {
            Roles = new List<ListItem>();
        }

        public IList<ListItem> Roles { get; set; }

        public static RoleListModel Get(IDb db)
        {
            var model = new RoleListModel
            {
                Roles = db.Roles
                    .OrderBy(r => r.Name)
                    .Select(r => new ListItem
                    {
                        Id = r.Id,
                        Name = r.Name
                    }).ToList()
            };

            foreach (var role in model.Roles)
            {
                role.UserCount = db.UserRoles
                    .Count(r => r.RoleId == role.Id);
            }
            return model;
        }

        public class ListItem
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public int UserCount { get; set; }
        }
    }
}