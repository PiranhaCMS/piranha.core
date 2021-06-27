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

namespace Piranha.AspNetCore.Identity.Models
{
    public class UserListModel
    {
        public IList<ListItem> Users { get; set; } = new List<ListItem>();

        public class ListItem
        {
            public Guid Id { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public IList<string> Roles { get; set; } = new List<string>();

            public string GravatarUrl { get; set; }
        }
    }
}