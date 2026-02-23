using Microsoft.AspNetCore.Identity;

namespace Aero.Identity.Models;

/// <summary>
/// RavenDB document model for a passkey credential.
/// </summary>
public class PasskeyCredential
{
    /// <summary>
    /// Gets or sets the unique identifier for the credential.
    /// </summary>
    public string CredentialId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the public key associated with the credential.
    /// </summary>
    public byte[] PublicKey { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Gets or sets the signature count for the credential.
    /// </summary>
    public long SignCount { get; set; }

    /// <summary>
    /// Gets or sets the user handle associated with the credential.
    /// </summary>
    public string UserHandle { get; set; } = string.Empty;
}
