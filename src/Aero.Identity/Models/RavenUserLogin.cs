namespace Aero.Identity.Models;

/// <summary>
/// RavenDB document model for a user login.
/// </summary>
public class RavenUserLogin
{
    /// <summary>
    /// Gets or sets the login provider (e.g. Facebook, Google).
    /// </summary>
    public string LoginProvider { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier for the user from the provider.
    /// </summary>
    public string ProviderKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name for the provider.
    /// </summary>
    public string ProviderDisplayName { get; set; } = string.Empty;
}

/// <summary>
/// RavenDB document model for a user claim.
/// </summary>
public class RavenUserClaim
{
    /// <summary>
    /// Gets or sets the claim type.
    /// </summary>
    public string ClaimType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the claim value.
    /// </summary>
    public string ClaimValue { get; set; } = string.Empty;
}
