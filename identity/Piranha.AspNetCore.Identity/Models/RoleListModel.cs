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

        public class ListItem
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public int UserCount { get; set; }
        }
    }
}