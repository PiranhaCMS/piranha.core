namespace Aero.Identity.Models;

/// <summary>
/// RavenDB document model for a passkey credential.
/// </summary>
public class PasskeyCredential
{
    /// <summary>
    /// Gets or sets the credential ID.
    /// </summary>
    public byte[] CredentialId { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Gets or sets the public key.
    /// </summary>
    public byte[] PublicKey { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Gets or sets the creation time.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the signature count.
    /// </summary>
    public uint SignCount { get; set; }

    /// <summary>
    /// Gets or sets the supported transports.
    /// </summary>
    public string[]? Transports { get; set; }

    /// <summary>
    /// Gets or sets whether the passkey is eligible for backup.
    /// </summary>
    public bool IsBackupEligible { get; set; }

    /// <summary>
    /// Gets or sets whether the passkey is currently backed up.
    /// </summary>
    public bool IsBackedUp { get; set; }

    /// <summary>
    /// Gets or sets whether the user is verified.
    /// </summary>
    public bool IsUserVerified { get; set; }

    /// <summary>
    /// Gets or sets the client data JSON.
    /// </summary>
    public byte[] ClientDataJson { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Gets or sets the attestation object.
    /// </summary>
    public byte[] AttestationObject { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Gets or sets the friendly name.
    /// </summary>
    public string? Name { get; set; }
}
