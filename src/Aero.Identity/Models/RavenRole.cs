using Microsoft.AspNetCore.Identity;

namespace Aero.Identity.Models;

/// <summary>
/// RavenDB document model for an identity role claim.
/// </summary>
public class RavenRoleClaim
{
    public string ClaimType { get; set; } = string.Empty;
    public string ClaimValue { get; set; } = string.Empty;
}

/// <summary>
/// RavenDB document model for an identity role.
/// </summary>
public class RavenRole : IdentityRole
{
    /// <summary>
    /// Gets or sets the list of claims associated with the role.
    /// </summary>
    public List<RavenRoleClaim> Claims { get; set; } = [];
}

