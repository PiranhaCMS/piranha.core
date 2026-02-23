using Microsoft.AspNetCore.Identity;

namespace Aero.Identity.Models;

/// <summary>
/// RavenDB document model for an identity user.
/// </summary>
public class RavenUser : IdentityUser
{
    /// <summary>
    /// Gets or sets the list of roles associated with the user.
    /// </summary>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of passkey credentials associated with the user.
    /// </summary>
    public List<PasskeyCredential> Passkeys { get; set; } = new();
}
