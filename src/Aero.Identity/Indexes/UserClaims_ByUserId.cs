using Aero.Identity.Models;
using Raven.Client.Documents.Indexes;
using System.Security.Claims;

namespace Aero.Identity.Indexes;

/// <summary>
/// Static index for retrieving all effective claims for a user.
/// Combines user claims with role claims for efficient authorization lookups.
/// </summary>
public class UserClaims_ByUserId : AbstractIndexCreationTask<RavenUser, UserClaims_ByUserId.Result>
{
    public class Result
    {
        public string UserId { get; set; } = string.Empty;
        public string ClaimType { get; set; } = string.Empty;
        public string ClaimValue { get; set; } = string.Empty;
    }

    public UserClaims_ByUserId()
    {
        Map = users => from user in users
                       from claim in user.Claims
                       select new Result
                       {
                           UserId = user.Id,
                           ClaimType = claim.ClaimType,
                           ClaimValue = claim.ClaimValue
                       };

        // Index for fast lookups by user ID
        Index(x => x.UserId, FieldIndexing.Exact);
        Index(x => x.ClaimType, FieldIndexing.Exact);
    }
}
