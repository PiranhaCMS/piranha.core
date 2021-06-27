/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System.Collections.Generic;
using Piranha.AspNetCore.Identity.Data;

namespace Piranha.AspNetCore.Identity.Models
{
    public class RoleEditModel
    {
        public RoleEditModel()
        {
            SelectedClaims = new List<string>();
        }

        public Role Role { get; set; }
        public IList<string> SelectedClaims { get; set; }
    }
}
