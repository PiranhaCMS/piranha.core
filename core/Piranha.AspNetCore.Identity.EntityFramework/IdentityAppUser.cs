using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Piranha.AspNetCore.Identity.EntityFramework
{
    public class IdentityAppUser : IdentityUser
    {
        public IdentityAppUser(params string[] claims) : this()
        {
            Claims.AddRange(claims);
        }

        public IdentityAppUser()
        {
            Claims = new List<string>();
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [NotMapped]
        public string Password { get; set; }

        [NotMapped]
        public List<string> Claims { get; set; }
    }
}