using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Piranha.AspNetCore.Identity.EntityFramework
{
    public class IdentityAppClaimsPrincipalFactory : UserClaimsPrincipalFactory<IdentityAppUser, IdentityRole>
    {
        public IdentityAppClaimsPrincipalFactory(UserManager<IdentityAppUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<IdentityOptions> options) : base(userManager, roleManager, options)
        {
        }

        public override async Task<ClaimsPrincipal> CreateAsync(IdentityAppUser user)
        {
            var principal = await base.CreateAsync(user);

            ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
            });

            return principal;
        }

    }
}