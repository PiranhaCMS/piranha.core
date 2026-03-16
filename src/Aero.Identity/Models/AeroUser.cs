using Microsoft.AspNetCore.Identity;

namespace Aero.Identity.Models;

/// <summary>
/// RavenDB document model for an identity user.
/// </summary>
public class AeroUser : IdentityUser
{
    /// <summary>
    /// Gets or sets the list of roles associated with the user.
    /// </summary>
    public List<string> Roles { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of external logins associated with the user.
    /// </summary>
    public List<RavenUserLogin> Logins { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of claims associated with the user.
    /// </summary>
    public List<RavenUserClaim> Claims { get; set; } = [];

    /// <summary>
    /// Gets or sets the authenticator key for the user.
    /// </summary>
    public string? AuthenticatorKey { get; set; }

    /// <summary>
    /// Gets or sets the semicolon-separated recovery codes for the user.
    /// </summary>
    public string? RecoveryCodes { get; set; }

    /// <summary>
    /// Gets or sets the list of passkey credentials associated with the user.
    /// </summary>
    public List<PasskeyCredential> Passkeys { get; set; } = [];
}
